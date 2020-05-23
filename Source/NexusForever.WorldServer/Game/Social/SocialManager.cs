﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NexusForever.WorldServer.Game.CharacterCache;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
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
        private const float LocalChatDistance = 155f;

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<ChatChannel, ChatChannelHandler> chatChannelHandlers
            = new Dictionary<ChatChannel, ChatChannelHandler>();
        private readonly Dictionary<ChatFormatType, ChatFormatFactoryDelegate> chatFormatFactories
            = new Dictionary<ChatFormatType, ChatFormatFactoryDelegate>();

        private delegate IChatFormat ChatFormatFactoryDelegate();
        private delegate void ChatChannelHandler(WorldSession session, ClientChat chat);

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
                Formats = ParseChatLinks(session, chat.Formats).ToList()
            };

            session.Player.Map.Search(
                session.Player.Position,
                LocalChatDistance,
                new SearchCheckRangePlayerOnly(session.Player.Position, LocalChatDistance, session.Player),
                out List<GridEntity> intersectedEntities
            );

            intersectedEntities.ForEach(e => ((Player)e).Session.EnqueueMessageEncrypted(serverChat));
            SendChatAccept(session);            
        }

        /// <summary>
        /// Handle's whisper messages between 2 clients
        /// </summary>
        public void HandleWhisperChat(WorldSession session, ClientChatWhisper whisper)
        {
            ICharacter character = CharacterManager.Instance.GetCharacterInfo(whisper.PlayerName);
            if (character == null || !(character is Player player))
            {
                SendMessage(session, $"Player \"{whisper.PlayerName}\" not found.");
                return;
            }

            if (session.Player.Name == character.Name)
            {
                SendMessage(session, "You cannot send a message to yourself.");
                return;
            }

            bool crossFactionChat = ConfigurationManager<WorldServerConfiguration>.Instance.Config.CrossFactionChat;
            if (session.Player.Faction1 != character.Faction1 && !crossFactionChat)
            {
                SendMessage(session, $"Player \"{whisper.PlayerName}\" not found.", "", ChatChannel.System);
                return;
            }

            // echo message
            session.EnqueueMessageEncrypted(new ServerChat
            {
                Channel      = ChatChannel.Whisper,
                Name         = whisper.PlayerName,
                Text         = whisper.Message,
                Self         = true,
                CrossFaction = session.Player.Faction1 != character.Faction1,
                Formats      = ParseChatLinks(session, whisper.Formats).ToList()
            });

            // target player message
            player.Session.EnqueueMessageEncrypted(new ServerChat
            {
                Channel      = ChatChannel.Whisper,
                Name         = session.Player.Name,
                Text         = whisper.Message,
                CrossFaction = session.Player.Faction1 != character.Faction1,
                Formats      = ParseChatLinks(session, whisper.Formats).ToList(),
            });
        }

        /// <summary>
        /// Parses chat links from <see cref="ChatFormat"/> delivered by <see cref="ClientChat"/>
        /// </summary>
        private IEnumerable<ChatFormat> ParseChatLinks(WorldSession session, IEnumerable<ChatFormat> formats)
        {
            return formats.Select(f => ParseChatFormat(session, f));
        }

        /// <summary>
        /// Parses a <see cref="ChatFormat"/> to return a formatted <see cref="ChatFormat"/>
        /// </summary>
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
        private static ChatFormat GetChatFormatForItem(ChatFormat chatFormat, Item item)
        {
            // TODO: this probably needs to be a full item response
            return new ChatFormat
            {
                Type        = ChatFormatType.ItemItemId,
                StartIndex  = chatFormat.StartIndex,
                StopIndex   = chatFormat.StopIndex,
                FormatModel = new ChatFormatItemId
                {
                    ItemId  = item.Entry.Id
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
