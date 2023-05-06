using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Social;
using NexusForever.Game.Configuration.Model;
using NexusForever.Game.Entity;
using NexusForever.Game.Static.RBAC;
using NexusForever.Game.Static.Social;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared.Configuration;

namespace NexusForever.Game.Social
{
    public partial class GlobalChatManager
    {
        [ChatChannelHandler(ChatChannelType.Say)]
        [ChatChannelHandler(ChatChannelType.Yell)]
        [ChatChannelHandler(ChatChannelType.Emote)]
        private void HandleLocalChat(IPlayer player, ClientChat chat)
        {
            var parser  = new ChatFormatter();
            var builder = new ChatMessageBuilder
            {
                Type     = chat.Channel.Type,
                FromName = player.Name,
                Text     = chat.Message,
                Formats  = parser.ParseChatLinks(player, chat.Formats).ToList(),
                Guid     = player.Guid,
                GM       = player.Account.RbacManager.HasPermission(Permission.GMFlag)
            };

            player.Talk(builder, LocalChatDistance, player);
            SendChatAccept(player);
        }

        [ChatChannelHandler(ChatChannelType.Guild)]
        [ChatChannelHandler(ChatChannelType.Society)]
        [ChatChannelHandler(ChatChannelType.WarParty)]
        [ChatChannelHandler(ChatChannelType.Community)]
        [ChatChannelHandler(ChatChannelType.GuildOfficer)]
        [ChatChannelHandler(ChatChannelType.WarPartyOfficer)]
        [ChatChannelHandler(ChatChannelType.Nexus)]
        [ChatChannelHandler(ChatChannelType.Trade)]
        [ChatChannelHandler(ChatChannelType.Custom)]
        private void HandleChannelChat(IPlayer player, ClientChat chat)
        {
            IChatChannel channel;
            ChatResult GetResult()
            {
                channel = GetChatChannel(chat.Channel.Type, chat.Channel.ChatId);
                if (channel == null)
                    return ChatResult.DoesntExist;

                return channel.CanBroadcast(player, chat.Message);
            }

            ChatResult result = GetResult();
            if (result != ChatResult.Ok)
            {
                SendChatResult(player.Session, chat.Channel.Type, chat.Channel.ChatId, result);
                return;
            }

            var parser  = new ChatFormatter();
            var builder = new ChatMessageBuilder
            {
                Type     = chat.Channel.Type,
                ChatId   = chat.Channel.ChatId,
                FromName = player.Name,
                Text     = chat.Message,
                Formats  = parser.ParseChatLinks(player, chat.Formats).ToList(),
                Guid     = player.Guid,
                GM       = player.Account.RbacManager.HasPermission(Permission.GMFlag)
            };

            channel.Broadcast(builder.Build(), player);
            SendChatAccept(player);
        }

        /// <summary>
        /// Handle's whisper messages between 2 clients
        /// </summary>
        public void HandleWhisperChat(IPlayer player, ClientChatWhisper whisper)
        {
            IPlayer target = PlayerManager.Instance.GetPlayer(whisper.PlayerName);

            bool CanWhisper()
            {
                if (target == null)
                    return false;

                if (player.Name == target.Name)
                    return false;

                bool crossFactionChat = SharedConfiguration.Instance.Get<RealmConfig>().CrossFactionChat;
                if (player.Faction1 != target.Faction1 && !crossFactionChat)
                    return false;

                return true;
            }

            if (!CanWhisper())
            {
                player.Session.EnqueueMessageEncrypted(new ServerChatWhisperFail
                {
                    CharacterTo      = whisper.PlayerName,
                    IsAccountWhisper = false,
                    Unknown1         = 1
                });
                return;
            }

            // target player message
            var parser  = new ChatFormatter();
            var builder = new ChatMessageBuilder
            {
                Type                 = ChatChannelType.Whisper,
                Self                 = false,
                FromName             = player.Name,
                Text                 = whisper.Message,
                Formats              = parser.ParseChatLinks(player, whisper.Formats).ToList(),
                CrossFaction         = player.Faction1 != target.Faction1,
                FromCharacterId      = player.CharacterId,
                FromCharacterRealmId = RealmContext.Instance.RealmId,
                GM                   = player.Account.RbacManager.HasPermission(Permission.GMFlag)
            };
            target.Session.EnqueueMessageEncrypted(builder.Build());

            SendChatAccept(player.Session, target);
        }
    }
}
