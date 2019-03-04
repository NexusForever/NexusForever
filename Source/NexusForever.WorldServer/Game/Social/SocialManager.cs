using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Network;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Map.Search;
using NexusForever.WorldServer.Game.Social.Model;
using NexusForever.WorldServer.Game.Social.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using NLog;
using Item = NexusForever.WorldServer.Game.Entity.Item;

namespace NexusForever.WorldServer.Game.Social
{
    public sealed class SocialManager : Singleton<SocialManager>
    {
        private const float LocalChatDistace = 155f;

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<ChatChannel, ChatChannelHandler> chatChannelHandlers
            = new Dictionary<ChatChannel, ChatChannelHandler>();
        private readonly Dictionary<ChatFormatType, ChatFormatFactoryDelegate> chatFormatFactories
            = new Dictionary<ChatFormatType, ChatFormatFactoryDelegate>();

        private delegate IChatFormat ChatFormatFactoryDelegate();
        private delegate void ChatChannelHandler(WorldSession session, ClientChat chat);

        // TODO: Switch to caching session GUIDs and announce to each session by Guid.
        private Dictionary<ChatChannel, List<WorldSession>> chatChannelSessions = new Dictionary<ChatChannel, List<WorldSession>>
        {
            { ChatChannel.Nexus, new List<WorldSession>() },
            { ChatChannel.Trade, new List<WorldSession>() }
        };
        private readonly bool CrossFactionChat = ConfigurationManager<WorldServerConfiguration>.Instance.Config.CrossFactionChat;

        private SocialManager()
        {
        }

        public void Initialise()
        {
            InitialiseChatHandlers();
            InitialiseChatFormatFactories();
        }

        private void InitialiseChatHandlers()
        {
            foreach (MethodInfo method in Assembly.GetExecutingAssembly().GetTypes()
                .SelectMany(t => t.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)))
            {
                IEnumerable<ChatChannelHandlerAttribute> attributes = method.GetCustomAttributes<ChatChannelHandlerAttribute>();
                foreach (ChatChannelHandlerAttribute attribute in attributes)
                {
                    #region Debug
                    ParameterInfo[] parameterInfo = method.GetParameters();
                    Debug.Assert(parameterInfo.Length == 2);
                    Debug.Assert(typeof(WorldSession) == parameterInfo[0].ParameterType);
                    Debug.Assert(typeof(ClientChat) == parameterInfo[1].ParameterType);
                    #endregion

                    ChatChannelHandler @delegate = (ChatChannelHandler)Delegate.CreateDelegate(typeof(ChatChannelHandler), this, method);
                    chatChannelHandlers.Add(attribute.ChatChannel, @delegate);
                }
            }
        }

        private void InitialiseChatFormatFactories()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                ChatFormatAttribute attribute = type.GetCustomAttribute<ChatFormatAttribute>();
                if (attribute == null)
                    continue;

                NewExpression @new = Expression.New(type.GetConstructor(Type.EmptyTypes));
                ChatFormatFactoryDelegate factory = Expression.Lambda<ChatFormatFactoryDelegate>(@new).Compile();
                chatFormatFactories.Add(attribute.Type, factory);
            }
        }

        /// <summary>
        /// Returns a new <see cref="IChatFormat"/> model for supplied <see cref="ChatFormatType"/> type.
        /// </summary>
        public IChatFormat GetChatFormatModel(ChatFormatType type)
        {
            if (!chatFormatFactories.TryGetValue(type, out ChatFormatFactoryDelegate factory))
                return null;
            return factory.Invoke();
        }

        /// <summary>
        /// Process and delegate a <see cref="ClientChat"/> message from <see cref="WorldSession"/>, this is called directly from a packet hander.
        /// </summary>
        public void HandleClientChat(WorldSession session, ClientChat chat)
        {
            if (chatChannelHandlers.ContainsKey(chat.Channel))
                chatChannelHandlers[chat.Channel](session, chat);
            else
            {
                log.Info($"ChatChannel {chat.Channel} has no handler implemented.");

                session.EnqueueMessageEncrypted(new ServerChat
                {
                    Channel = ChatChannel.Debug,
                    Name    = "SocialManager",
                    Text    = "Currently not implemented",
                });
            }
        }

        private void SendChatAccept(WorldSession session)
        {
            session.EnqueueMessageEncrypted(new ServerChatAccept
            {
                Name = session.Player.Name,
                Guid = session.Player.Guid
            });
        }

        /// <summary>
        /// Add the <see cref="WorldSession"/> to the chat channels sessions list for appropriate chat channels.
        /// </summary>
        /// <param name="session"></param>
        public void JoinChatChannels(WorldSession session)
        {
            foreach(KeyValuePair<ChatChannel, List<WorldSession>> chatChannel in chatChannelSessions)
            {
                if (chatChannelSessions[chatChannel.Key].Contains(session) || chatChannelSessions[chatChannel.Key].FindAll(s => s.Player == session.Player).ToList().Count <= 0)
                    chatChannelSessions[chatChannel.Key].Remove(session);

                chatChannelSessions[chatChannel.Key].Add(session);

                session.EnqueueMessageEncrypted(new ServerChatJoin
                {
                    Channel = chatChannel.Key
                });
            }
        }

        /// <summary>
        /// Removes the <see cref="WorldSession"/> from appropriate chat channels.
        /// </summary>
        /// <param name="session"></param>
        public void LeaveChatChannels(WorldSession session)
        {
            foreach (KeyValuePair<ChatChannel, List<WorldSession>> chatChannel in chatChannelSessions)
                chatChannelSessions[chatChannel.Key].Remove(session);
        }

        [ChatChannelHandler(ChatChannel.Say)]
        [ChatChannelHandler(ChatChannel.Yell)]
        [ChatChannelHandler(ChatChannel.Emote)]
        private void HandleLocalChat(WorldSession session, ClientChat chat)
        {
            var serverChat = new ServerChat
            {
                Guid    = session.Player.Guid,
                Channel = chat.Channel,
                Name    = session.Player.Name,
                Text    = chat.Message,
                Formats = ParseChatLinks(session, chat).ToList(),
            };

            session.Player.Map.Search(
                session.Player.Position,
                LocalChatDistace,
                new SearchCheckRangePlayerOnly(session.Player.Position, LocalChatDistace, session.Player),
                out List<GridEntity> intersectedEntities
            );

            intersectedEntities.ForEach(e => ((Player)e).Session.EnqueueMessageEncrypted(serverChat));
            SendChatAccept(session);            
        }

        /// <summary>
        /// Handle's whisper messages between 2 clients
        /// </summary>
        /// <param name="session"></param>
        /// <param name="whisper"></param>
        public void HandleWhisperChat(WorldSession session, ClientChatWhisper whisper)
        {
            WorldSession targetSession = NetworkManager<WorldSession>.Instance.GetSession(s => s.Player?.Name == whisper.PlayerName);
            if (targetSession != null)
            {
                if (targetSession == session)
                {
                    SendMessage(session, $"You cannot send a message to yourself.", "", ChatChannel.System);
                    return;
                }

                if (targetSession.Player.Faction1 != session.Player.Faction1 && !CrossFactionChat)
                {
                    SendMessage(session, $"Player \"{whisper.PlayerName}\" not found.", "", ChatChannel.System);
                    return;
                }

                // Echo Message
                session.EnqueueMessageEncrypted(new ServerChat
                {
                    Channel = ChatChannel.Whisper,
                    Name = whisper.PlayerName,
                    Text = whisper.Message,
                    Self = true,
                    CrossFaction = targetSession.Player.Faction1 != session.Player.Faction1,
                    Formats = ParseChatLinks(session, whisper).ToList(),
                });

                // Target Player Message
                targetSession.EnqueueMessageEncrypted(new ServerChat
                {
                    Channel = ChatChannel.Whisper,
                    Name = session.Player.Name,
                    Text = whisper.Message,
                    CrossFaction = targetSession.Player.Faction1 != session.Player.Faction1,
                    Formats = ParseChatLinks(session, whisper).ToList(),
                });
            }
            else
            {
                SendMessage(session, $"Player \"{whisper.PlayerName}\" not found.", "", ChatChannel.System);
            }
        }

        /// <summary>
        /// Handles server-wide <see cref="ChatChannel"/>
        /// </summary>
        /// <param name="session"></param>
        /// <param name="chat"></param>
        [ChatChannelHandler(ChatChannel.Nexus)]
        [ChatChannelHandler(ChatChannel.Trade)]
        private void HandleChannelChat(WorldSession session, ClientChat chat)
        {
            var serverChat = new ServerChat
            {
                Guid = session.Player.Guid,
                Channel = chat.Channel,
                Name = session.Player.Name,
                Text = chat.Message,
                Formats = ParseChatLinks(session, chat).ToList(),
            };

            foreach (WorldSession channelSession in chatChannelSessions[chat.Channel])
            {
                serverChat.CrossFaction = session.Player.Faction1 != channelSession.Player.Faction1;
                channelSession.EnqueueMessageEncrypted(serverChat);
            }
        }

        /// <summary>
        /// Parses chat links from <see cref="ChatFormat"/> delivered by <see cref="ClientChat"/>
        /// </summary>
        /// <param name="session"></param>
        /// <param name="chat"></param>
        /// <returns></returns>
        private IEnumerable<ChatFormat> ParseChatLinks(WorldSession session, ClientChat chat)
        {
            foreach (ChatFormat format in chat.Formats)
            {
                yield return ParseChatFormat(session, format);
            }
        }

        /// <summary>
        /// Parses chat links from <see cref="ChatFormat"/> delivered by <see cref="ClientChatWhisper"/>
        /// </summary>
        /// <param name="session"></param>
        /// <param name="chat"></param>
        /// <returns></returns>
        private IEnumerable<ChatFormat> ParseChatLinks(WorldSession session, ClientChatWhisper chat)
        {
            foreach (ChatFormat format in chat.Formats)
            {
                yield return ParseChatFormat(session, format);
            }
        }

        /// <summary>
        /// Parses a <see cref="ChatFormat"/> to return a formatted <see cref="ChatFormat"/>
        /// </summary>
        /// <param name="session"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        private ChatFormat ParseChatFormat(WorldSession session, ChatFormat format)
        {
            switch (format.FormatModel)
            {
                case ChatFormatItemGuid chatFormatItemGuid:
                    {
                        Item item = session.Player.Inventory.GetItem(chatFormatItemGuid.Guid);

                        return GetChatFormatForItem(format, item);
                    }
                default:
                    return format;
            }
        }

        /// <summary>
        /// Returns formatted <see cref="ChatFormat"/> for an Item Link
        /// </summary>
        /// <param name="chatFormat"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private ChatFormat GetChatFormatForItem(ChatFormat chatFormat, Item item)
        {
            // TODO: this probably needs to be a full item response
            return new ChatFormat
            {
                Type = ChatFormatType.ItemItemId,
                StartIndex = chatFormat.StartIndex,
                StopIndex = chatFormat.StopIndex,
                FormatModel = new ChatFormatItemId
                {
                    ItemId = item.Entry.Id
                }
            };
        }

        public void SendMessage(WorldSession session, string message, string name = "", ChatChannel channel = ChatChannel.System)
        {
           session.EnqueueMessageEncrypted(new ServerChat
            {
                Channel = channel,
                Name    = name,
                Text    = message,
            });
        }
    }
}
