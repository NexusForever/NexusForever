using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using NexusForever.Database.Character.Model;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Database;
using NexusForever.Shared.Game;
using NexusForever.WorldServer.Game.CharacterCache;
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
    public sealed class GlobalChatManager : Singleton<GlobalChatManager>, IUpdate
    {
        private const float LocalChatDistance = 155f;

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<ChatChannelType, ChatChannelHandler> chatChannelHandlers = new();
        private readonly Dictionary<ChatFormatType, ChatFormatFactoryDelegate> chatFormatFactories = new();

        private delegate IChatFormat ChatFormatFactoryDelegate();
        private delegate void ChatChannelHandler(WorldSession session, ClientChat chat);

        private readonly Dictionary<ChatChannelType, Dictionary<ulong, ChatChannel>> chatChannels = new();
        private readonly Dictionary<ChatChannelType, ulong> chatChannelIds = new();
        private readonly Dictionary<ChatChannelType, Dictionary<string, ulong>> chatChannelNames = new();
        private readonly Dictionary<ChatChannelType, Dictionary<ulong, List<ulong>>> characterChatChannels = new();

        private readonly UpdateTimer saveTimer = new(60d);

        private GlobalChatManager()
        {
        }

        public void Initialise()
        {
            foreach (ChatChannelType type in Enum.GetValues(typeof(ChatChannelType)))
            {
                chatChannels.Add(type, new Dictionary<ulong, ChatChannel>());
                chatChannelIds.Add(type, 1);
                chatChannelNames.Add(type, new Dictionary<string, ulong>(StringComparer.InvariantCultureIgnoreCase));
                characterChatChannels.Add(type, new Dictionary<ulong, List<ulong>>());
            }

            InitialiseChatChannels();

            InitialiseChatHandlers();
            InitialiseChatFormatFactories();
        }

        private void InitialiseChatChannels()
        {
            foreach (ChatChannelModel channelModel in DatabaseManager.Instance.CharacterDatabase.GetChatChannels())
            {
                var chatChannel = new ChatChannel(channelModel);
                foreach (ChatChannelMember member in chatChannel)
                    TrackCharacterChatChannel(member.CharacterId, chatChannel.Type, chatChannel.Id);

                chatChannels[chatChannel.Type].Add(chatChannel.Id, chatChannel);
                chatChannelNames[chatChannel.Type].Add(chatChannel.Name, chatChannel.Id);
            }
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

                ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                if (constructor == null)
                    continue;

                NewExpression @new = Expression.New(constructor);
                ChatFormatFactoryDelegate factory = Expression.Lambda<ChatFormatFactoryDelegate>(@new).Compile();
                chatFormatFactories.Add(attribute.Type, factory);
            }
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            saveTimer.Update(lastTick);

            if (saveTimer.HasElapsed)
            {
                UpdateCustomChannels();
                saveTimer.Reset();
            }
        }

        private void UpdateCustomChannels()
        {
            var tasks = new List<Task>();
            foreach (ChatChannel channel in chatChannels[ChatChannelType.Custom].Values.ToList())
            {
                if (channel.PendingDelete)
                {
                    chatChannels[ChatChannelType.Custom].Remove(channel.Id);
                    chatChannelNames[ChatChannelType.Custom].Remove(channel.Name);

                    if (channel.PendingCreate)
                        continue;
                }

                tasks.Add(DatabaseManager.Instance.CharacterDatabase.Save(channel.Save));
            }

            Task.WaitAll(tasks.ToArray());
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
        /// Create a new <see cref="ChatChannel"/> with supplied <see cref="ChatChannelType"/> and id.
        /// </summary>
        public ChatChannel CreateChatChannel(ChatChannelType type, string name, string password = null)
        {
            if (chatChannelNames[type].TryGetValue(name, out ulong chatId))
            {
                if (!chatChannels[type].TryGetValue(chatId, out ChatChannel channel) || !channel.PendingDelete)
                    throw new InvalidOperationException($"Chat channel {type},{name} already exists!");

                channel.EnqueueDelete(false);
                channel.Password = password;
                return channel;
            }
            else
            {
                ulong id = chatChannelIds[type]++;
                var channel = new ChatChannel(type, id, name, password);
                chatChannels[type].Add(id, channel);
                chatChannelNames[type].Add(name, id);
                return channel;
            }
        }

        /// <summary>
        /// Returns an existing <see cref="ChatChannel"/> with supplied <see cref="ChatChannelType"/> and id.
        /// </summary>
        public ChatChannel GetChatChannel(ChatChannelType type, ulong id)
        {
            if (chatChannels[type].TryGetValue(id, out ChatChannel channel) && !channel.PendingDelete)
                return channel;

            return null;
        }

        /// <summary>
        /// Returns an existing <see cref="ChatChannel"/> with supplied <see cref="ChatChannelType"/> and name.
        /// </summary>
        public ChatChannel GetChatChannel(ChatChannelType type, string name)
        {
            if (!chatChannelNames[type].TryGetValue(name, out ulong chatId))
                return null;

            return GetChatChannel(type, chatId);
        }

        /// <summary>
        /// Returns a collection of <see cref="ChatChannel"/>'s in which supplied character id belongs to.
        /// </summary>
        /// <remarks>
        /// This should only be used in situations where the local <see cref="ChatManager"/> is not accessible for a character.
        /// </remarks>
        public IEnumerable<ChatChannel> GetCharacterChatChannels(ChatChannelType type, ulong characterId)
        {
            characterChatChannels[type].TryGetValue(characterId, out List<ulong> channels);
            foreach (ulong chatId in channels ?? Enumerable.Empty<ulong>())
                yield return GetChatChannel(type, chatId);
        }

        /// <summary>
        /// Track a new chat channel for the supplied character.
        /// </summary>
        /// <remarks>
        /// Used to notify the global manager that a local manager is tracking a new guild.
        /// </remarks>
        public void TrackCharacterChatChannel(ulong characterId, ChatChannelType type, ulong chatId)
        {
            if (!characterChatChannels[type].TryGetValue(characterId, out List<ulong> channels))
            {
                channels = new List<ulong>();
                characterChatChannels[type].Add(characterId, channels);
            }

            channels.Add(chatId);
        }

        /// <summary>
        /// Stop tracking an existing chat channel for the supplied character.
        /// </summary>
        /// <remarks>
        /// Used to notify the global manager that a local manager has stopped tracking an existing guild.
        /// </remarks>
        public void UntrackCharacterChatChannel(ulong characterId, ChatChannelType type, ulong chatId)
        {
            if (characterChatChannels[type].TryGetValue(characterId, out List<ulong> channels))
                channels.Remove(chatId);
        }

        /// <summary>
        /// Process and delegate a <see cref="ClientChat"/> message from <see cref="WorldSession"/>, this is called directly from a packet hander.
        /// </summary>
        public void HandleClientChat(WorldSession session, ClientChat chat)
        {
            if (chatChannelHandlers.ContainsKey(chat.Channel.Type))
                chatChannelHandlers[chat.Channel.Type](session, chat);
            else
            {
                log.Info($"ChatChannel {chat.Channel} has no handler implemented.");
                SendMessage(session, "Currently not implemented", "GlobalChatManager", ChatChannelType.Debug);
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

        [ChatChannelHandler(ChatChannelType.Say)]
        [ChatChannelHandler(ChatChannelType.Yell)]
        [ChatChannelHandler(ChatChannelType.Emote)]
        private void HandleLocalChat(WorldSession session, ClientChat chat)
        {
            var builder = new ChatMessageBuilder
            {
                Type     = chat.Channel.Type,
                FromName = session.Player.Name,
                Text     = chat.Message,
                Formats  = ParseChatLinks(session, chat.Formats).ToList(),
                Guid     = session.Player.Guid
            };

            session.Player.Map.Search(
                session.Player.Position,
                LocalChatDistance,
                new SearchCheckRangePlayerOnly(session.Player.Position, LocalChatDistance, session.Player),
                out List<GridEntity> intersectedEntities
            );

            var serverChat = builder.Build();
            intersectedEntities.ForEach(e => ((Player)e).Session.EnqueueMessageEncrypted(serverChat));
            SendChatAccept(session);
        }

        [ChatChannelHandler(ChatChannelType.Guild)]
        [ChatChannelHandler(ChatChannelType.Society)]
        [ChatChannelHandler(ChatChannelType.WarParty)]
        [ChatChannelHandler(ChatChannelType.Community)]
        [ChatChannelHandler(ChatChannelType.GuildOfficer)]
        [ChatChannelHandler(ChatChannelType.WarPartyOfficer)]
        [ChatChannelHandler(ChatChannelType.Custom)]
        private void HandleChannelChat(WorldSession session, ClientChat chat)
        {
            ChatChannel channel;
            ChatResult GetResult()
            {
                channel = GetChatChannel(chat.Channel.Type, chat.Channel.ChatId);
                if (channel == null)
                    return ChatResult.DoesntExist;

                return channel.CanBroadcast(session.Player, chat.Message);
            }

            ChatResult result = GetResult();
            if (result != ChatResult.Ok)
            {
                SendChatResult(session, chat.Channel.Type, chat.Channel.ChatId, result);
                return;
            }

            var builder = new ChatMessageBuilder
            {
                Type     = chat.Channel.Type,
                ChatId   = chat.Channel.ChatId,
                FromName = session.Player.Name,
                Text     = chat.Message,
                Formats  = ParseChatLinks(session, chat.Formats).ToList(),
                Guid     = session.Player.Guid
            };

            channel.Broadcast(builder.Build());
        }

        /// <summary>
        /// Handle's whisper messages between 2 clients
        /// </summary>
        public void HandleWhisperChat(WorldSession session, ClientChatWhisper whisper)
        {
            Player target = CharacterManager.Instance.GetPlayer(whisper.PlayerName);
            if (target == null)
            {
                SendMessage(session, $"Player \"{whisper.PlayerName}\" not found.");
                return;
            }

            if (session.Player.Name == target.Name)
            {
                SendMessage(session, "You cannot send a message to yourself.");
                return;
            }

            bool crossFactionChat = ConfigurationManager<WorldServerConfiguration>.Instance.Config.CrossFactionChat;
            if (session.Player.Faction1 != target.Faction1 && !crossFactionChat)
            {
                SendMessage(session, $"Player \"{whisper.PlayerName}\" not found.");
                return;
            }

            // echo message
            var builder = new ChatMessageBuilder
            {
                Type         = ChatChannelType.Whisper,
                Self         = true,
                FromName     = whisper.PlayerName,
                Text         = whisper.Message,
                Formats      = ParseChatLinks(session, whisper.Formats).ToList(),
                CrossFaction = session.Player.Faction1 != target.Faction1
            };
            session.EnqueueMessageEncrypted(builder.Build());

            // target player message
            builder.Self = false;
            target.Session.EnqueueMessageEncrypted(builder.Build());
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

        public void SendMessage(WorldSession session, string message, string name = "", ChatChannelType type = ChatChannelType.System)
        {
            var builder = new ChatMessageBuilder
            {
                Type     = type,
                FromName = name,
                Text     = message
            };
            session.EnqueueMessageEncrypted(builder.Build());
        }

        public void SendChatResult(WorldSession session, ChatChannelType type, ulong chatId, ChatResult result)
        {
            session.EnqueueMessageEncrypted(new ServerChatResult
            {
                Channel    = new Channel
                {
                    Type   = type,
                    ChatId = chatId
                },
                ChatResult = result
            });
        }
    }
}
