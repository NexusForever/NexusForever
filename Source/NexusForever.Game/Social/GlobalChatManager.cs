using System.Diagnostics;
using System.Reflection;
using NexusForever.Database;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Social;
using NexusForever.Game.Static.RBAC;
using NexusForever.Game.Static.Social;
using NexusForever.Network;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Shared;
using NexusForever.Shared.Game;
using NLog;

namespace NexusForever.Game.Social
{
    public sealed partial class GlobalChatManager : Singleton<GlobalChatManager>, IGlobalChatManager
    {
        private const float LocalChatDistance = 155f;

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<ChatChannelType, ChatChannelHandler> chatChannelHandlers = new();
        private delegate void ChatChannelHandler(IPlayer player, ClientChat chat);

        private readonly Dictionary<ChatChannelType, Dictionary<ulong, IChatChannel>> chatChannels = new();
        private readonly Dictionary<ChatChannelType, ulong> chatChannelIds = new();
        private readonly Dictionary<ChatChannelType, Dictionary<string, ulong>> chatChannelNames = new();
        private readonly Dictionary<ChatChannelType, Dictionary<ulong, List<ulong>>> characterChatChannels = new();

        private readonly List<ChatChannelType> defaultChannelTypes = new()
        {
            ChatChannelType.Nexus,
            ChatChannelType.Trade,
        };

        private readonly UpdateTimer saveTimer = new(60d);

        public void Initialise()
        {
            foreach (ChatChannelType type in Enum.GetValues(typeof(ChatChannelType)))
            {
                chatChannels.Add(type, new Dictionary<ulong, IChatChannel>());
                chatChannelIds.Add(type, 1);
                chatChannelNames.Add(type, new Dictionary<string, ulong>(StringComparer.InvariantCultureIgnoreCase));
                characterChatChannels.Add(type, new Dictionary<ulong, List<ulong>>());
            }

            InitialiseChatChannels();

            InitialiseChatHandlers();
        }

        private void InitialiseChatChannels()
        {
            foreach (ChatChannelModel channelModel in DatabaseManager.Instance.GetDatabase<CharacterDatabase>().GetChatChannels())
            {
                var chatChannel = new ChatChannel(channelModel);
                foreach (IChatChannelMember member in chatChannel)
                    TrackCharacterChatChannel(member.CharacterId, chatChannel.Type, chatChannel.Id);

                chatChannels[chatChannel.Type].Add(chatChannel.Id, chatChannel);
                chatChannelNames[chatChannel.Type].Add(chatChannel.Name, chatChannel.Id);
            }

            foreach (ChatChannelType channelType in defaultChannelTypes)
                CreateChatChannel(channelType, 1, channelType.ToString());
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
                    Debug.Assert(typeof(IPlayer) == parameterInfo[0].ParameterType);
                    Debug.Assert(typeof(ClientChat) == parameterInfo[1].ParameterType);
                    #endregion

                    ChatChannelHandler @delegate = (ChatChannelHandler)Delegate.CreateDelegate(typeof(ChatChannelHandler), this, method);
                    chatChannelHandlers.Add(attribute.ChatChannel, @delegate);
                }
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
            foreach (IChatChannel channel in chatChannels[ChatChannelType.Custom].Values.ToList())
            {
                if (channel.PendingDelete)
                {
                    chatChannels[ChatChannelType.Custom].Remove(channel.Id);
                    chatChannelNames[ChatChannelType.Custom].Remove(channel.Name);

                    if (channel.PendingCreate)
                        continue;
                }

                tasks.Add(DatabaseManager.Instance.GetDatabase<CharacterDatabase>().Save(channel.Save));
            }

            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// Create a new <see cref="IChatChannel"/> with supplied <see cref="ChatChannelType"/>.
        /// </summary>
        public IChatChannel CreateChatChannel(ChatChannelType type, string name, string password = null)
        {
            if (chatChannelNames[type].TryGetValue(name, out ulong chatId))
            {
                if (!chatChannels[type].TryGetValue(chatId, out IChatChannel channel) || !channel.PendingDelete)
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
        /// Create a new <see cref="IChatChannel"/> with supplied <see cref="ChatChannelType"/> and id.
        /// </summary>
        public IChatChannel CreateChatChannel(ChatChannelType type, ulong chatId, string name, string password = null)
        {
            if (chatChannels[type].TryGetValue(chatId, out IChatChannel channel))
            {
                if (!channel.PendingDelete)
                    throw new InvalidOperationException($"Chat channel {type},{name} already exists!");

                channel.EnqueueDelete(false);
                channel.Password = password;   
            }
            else
            {
                channel = new ChatChannel(type, chatId, name, password);
                chatChannels[type].Add(chatId, channel);
                chatChannelNames[type].Add(name, chatId);
            }

            return channel;
        }

        /// <summary>
        /// Returns an existing <see cref="IChatChannel"/> with supplied <see cref="ChatChannelType"/> and id.
        /// </summary>
        public IChatChannel GetChatChannel(ChatChannelType type, ulong id)
        {
            if (chatChannels[type].TryGetValue(id, out IChatChannel channel) && !channel.PendingDelete)
                return channel;

            return null;
        }

        /// <summary>
        /// Returns an existing <see cref="IChatChannel"/> with supplied <see cref="ChatChannelType"/> and name.
        /// </summary>
        public IChatChannel GetChatChannel(ChatChannelType type, string name)
        {
            if (!chatChannelNames[type].TryGetValue(name, out ulong chatId))
                return null;

            return GetChatChannel(type, chatId);
        }

        /// <summary>
        /// Returns a collection of <see cref="IChatChannel"/>'s in which supplied character id belongs to.
        /// </summary>
        /// <remarks>
        /// This should only be used in situations where the local manager is not accessible for a character.
        /// </remarks>
        public IEnumerable<IChatChannel> GetCharacterChatChannels(ChatChannelType type, ulong characterId)
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
        /// Process and delegate a <see cref="ClientChat"/> message from <see cref="IPlayer"/>.
        /// </summary>
        public void HandleClientChat(IPlayer player, ClientChat chat)
        {
            if (chatChannelHandlers.TryGetValue(chat.Channel.Type, out ChatChannelHandler handler))
                handler(player, chat);
            else
            {
                log.Info($"ChatChannel {chat.Channel} has no handler implemented.");
                SendMessage(player.Session, "Currently not implemented", "GlobalChatManager", ChatChannelType.Debug);
            }
        }

        private void SendChatAccept(IPlayer player)
        {
            player.Session.EnqueueMessageEncrypted(new ServerChatAccept
            {
                Name = player.Name,
                Guid = player.Guid,
                GM   = player.Account.RbacManager.HasPermission(Permission.GMFlag)
            });
        }

        private void SendChatAccept(IGameSession session, IPlayer target)
        {
            session.EnqueueMessageEncrypted(new ServerChatAccept
            {
                Name = target.Name,
                Guid = target.Guid,
                GM   = target.Account.RbacManager.HasPermission(Permission.GMFlag)
            });
        }

        /// <summary>
        /// Add the <see cref="IPlayer"/> to the chat channels sessions list for appropriate chat channels.
        /// </summary>
        public void JoinDefaultChatChannels(IPlayer player)
        {
            foreach (ChatChannelType channelType in defaultChannelTypes)
                GetChatChannel(channelType, 1)?.Join(player, null);
        }

        /// <summary>
        /// Remove the <see cref="IPlayer"/> from the chat channels sessions list for appropriate chat channels.
        /// </summary>
        public void LeaveDefaultChatChannels(IPlayer player)
        {
            foreach (ChatChannelType channelType in defaultChannelTypes)
                GetChatChannel(channelType, 1)?.Leave(player.CharacterId);
        }

        public void SendMessage(IGameSession session, string message, string name = "", ChatChannelType type = ChatChannelType.System)
        {
            var builder = new ChatMessageBuilder
            {
                Type     = type,
                FromName = name,
                Text     = message
            };
            session.EnqueueMessageEncrypted(builder.Build());
        }

        public void SendChatResult(IGameSession session, ChatChannelType type, ulong chatId, ChatResult result)
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
