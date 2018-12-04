﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Map;
using NexusForever.WorldServer.Game.Social.Model;
using NexusForever.WorldServer.Game.Social.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using NLog;

namespace NexusForever.WorldServer.Game.Social
{
    public class SocialManager
    {
        private const float LocalChatDistace = 155f;

        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private static readonly Dictionary<ChatChannel, ChatChannelHandler> chatChannelHandlers
            = new Dictionary<ChatChannel, ChatChannelHandler>();
        private static readonly Dictionary<ChatFormatType, ChatFormatFactoryDelegate> chatFormatFactories
            = new Dictionary<ChatFormatType, ChatFormatFactoryDelegate>();

        private delegate IChatFormat ChatFormatFactoryDelegate();
        private delegate void ChatChannelHandler(WorldSession session, ClientChat chat);

        public static void Initialise()
        {
            InitialiseChatHandlers();
            InitialiseChatFormatFactories();
        }

        private static void InitialiseChatHandlers()
        {
            IEnumerable<MethodInfo> methods = Assembly.GetExecutingAssembly()
                .GetTypes()
                .SelectMany(t => t.GetMethods(BindingFlags.NonPublic | BindingFlags.Static));

            foreach (MethodInfo method in methods)
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

                    ChatChannelHandler @delegate = (ChatChannelHandler)Delegate.CreateDelegate(typeof(ChatChannelHandler), method);
                    chatChannelHandlers.Add(attribute.ChatChannel, @delegate);
                }
            }
        }

        private static void InitialiseChatFormatFactories()
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
        public static IChatFormat GetChatFormatModel(ChatFormatType type)
        {
            if (!chatFormatFactories.TryGetValue(type, out ChatFormatFactoryDelegate factory))
                return null;
            return factory.Invoke();
        }

        /// <summary>
        /// Process and delegate a <see cref="ClientChat"/> message from <see cref="WorldSession"/>, this is called directly from a packet hander.
        /// </summary>
        public static void HandleClientChat(WorldSession session, ClientChat chat)
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

        private static void SendChatAccept(WorldSession session)
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
        private static void HandleLocalChat(WorldSession session, ClientChat chat)
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

        private static IEnumerable<ChatFormat> ParseChatLinks(WorldSession session, ClientChat chat)
        {
            foreach (ChatFormat format in chat.Formats)
            {
                switch (format.FormatModel)
                {
                    case ChatFormatItemGuid chatFormatItemGuid:
                    {
                        var item = session.Player.Inventory.GetItemByGuid(chatFormatItemGuid.Guid);

                        // TODO: this probably needs to be a full item response
                        yield return new ChatFormat
                        {
                            Type        = ChatFormatType.ItemItemId,
                            StartIndex  = 0,
                            StopIndex   = 0,
                            FormatModel = new ChatFormatItemId
                            {
                                ItemId = item.Entry.Id
                            }
                        };
                        break;
                    }
                    default:
                        yield return format;
                        break;
                }
            }
        }

        public static void SendMessage(WorldSession session, string message, string name = "", ChatChannel channel = ChatChannel.System)
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
