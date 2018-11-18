using System;
using System.Collections.Generic;
using System.Text;
using NexusForever.WorldServer.Database.Character;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Map;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using NLog;
using System.Linq;

namespace NexusForever.WorldServer.Game.Social
{
    public class SocialManager
    {
        private static readonly float LocalChatDistace = 155;
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();
        private static Dictionary<ChatChannel, ChatChannelHandler> ChatChannelHandlers = new Dictionary<ChatChannel, ChatChannelHandler>();

        private delegate void ChatChannelHandler(WorldSession session, ClientChat chat);

        public static void Initialise()
        {
            ChatChannelHandlers.Add(ChatChannel.Say, HandleLocalChat);
            ChatChannelHandlers.Add(ChatChannel.Yell, HandleLocalChat);
            ChatChannelHandlers.Add(ChatChannel.Emote, HandleLocalChat);
        }

        public static void HandleClientChat(WorldSession session, ClientChat chat)
        {
            if(ChatChannelHandlers.ContainsKey(chat.Channel))
            {
                ChatChannelHandlers[chat.Channel](session, chat);
            }
            else
            {
                log.Info($"ChatChannel {chat.Channel} has no handler implemented.");

                session.EnqueueMessageEncrypted(new ServerChat
                {
                    Channel = ChatChannel.Debug,
                    Name = "SocialManager",
                    Text = "Currently not implemented",
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

        private static void HandleLocalChat(WorldSession session, ClientChat chat)
        {
            ServerChat chatMessage = new ServerChat
            {
                Guid = session.Player.Guid,
                Channel = chat.Channel,
                Name = session.Player.Name,
                Text = chat.Message,
                LinkedItems = ParseChatLinks(session, chat.LinkItems),
            };

            session.Player.Map.Search(
                session.Player.Position,
                LocalChatDistace,
                new SearchCheckRangeExcludeGridEntity(session.Player.Position, LocalChatDistace, session.Player),
                out List<GridEntity> intersectedEntities
            );
            
            foreach (GridEntity entity in intersectedEntities)
            {
                if (entity is Player player)
                {
                    player.Session.EnqueueMessageEncrypted(chatMessage);
                }
            }

            SendChatAccept(session);            
        }

        public static List<ServerLinkedItemId> ParseChatLinks(WorldSession session, List<ClientChat.ChatLink> chatLinks)
        {
            List<ServerLinkedItemId> outgoingList = new List<ServerLinkedItemId>();

            chatLinks.ForEach((chatLink) =>
            {
                if (chatLink is ClientChat.ItemGuidChatLink itemGuidChatLink)
                {
                    var item = session.Player.Inventory.GetItemByGuid(itemGuidChatLink.Guid);
                    
                    outgoingList.Add(new ServerLinkedItemId
                    {
                        StartIndex = chatLink.StartIndex,
                        EndIndex = chatLink.EndIndex,
                        ItemId = item.Entry.Id
                    });                     
                }
                else if (chatLink is ClientChat.ItemIdChatLink itemIdChatLink)
                {
                    outgoingList.Add(new ServerLinkedItemId
                    {
                        StartIndex = chatLink.StartIndex,
                        EndIndex = chatLink.EndIndex,
                        ItemId = itemIdChatLink.ItemId
                    });
                }
                else
                {
                    log.Info($"Unable to parse link.");
                }
            });

            return outgoingList;
        }

        public static void SendMessage(WorldSession session, string message, string name = "", ChatChannel channel = ChatChannel.System)
        {
           session.EnqueueMessageEncrypted(new ServerChat
            {
                Channel = ChatChannel.System,
                Name = name,
                Text = message,
            });
        }
    }
}
