using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Chat;
using NexusForever.Game.Abstract.Chat.Format;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.RBAC;
using NexusForever.Game.Static.Chat;
using NexusForever.GameTable.Text.Filter;
using NexusForever.GameTable.Text.Static;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Chat;
using NexusForever.Network.Session;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Chat;
using NexusForever.Shared;

namespace NexusForever.Game.Chat
{
    public sealed class GlobalChatManager : Singleton<GlobalChatManager>, IGlobalChatManager
    {
        private const float LocalChatDistance = 155f;

        private bool IsLocalChat(ChatChannelType type) => type
            is ChatChannelType.Say
            or ChatChannelType.Yell
            or ChatChannelType.Emote;

        private bool IsInternalChat(ChatChannelType type) => type
            is ChatChannelType.Party
            or ChatChannelType.Instance
            or ChatChannelType.Nexus
            or ChatChannelType.Trade
            or ChatChannelType.Custom
            or ChatChannelType.Guild
            or ChatChannelType.Society
            or ChatChannelType.WarParty
            or ChatChannelType.Community
            or ChatChannelType.GuildOfficer
            or ChatChannelType.WarPartyOfficer;

        #region Dependency Injection

        private readonly ILogger<GlobalChatManager> log;

        private readonly IChatFormatManager chatFormatManager;
        private readonly ITextFilterManager textFilterManager;
        private readonly IInternalMessagePublisher messagePublisher;

        public GlobalChatManager(
            ILogger<GlobalChatManager> log,
            IChatFormatManager chatFormatManager,
            ITextFilterManager textFilterManager,
            IInternalMessagePublisher messagePublisher)
        {
            this.log               = log;
            this.chatFormatManager = chatFormatManager;
            this.textFilterManager = textFilterManager;
            this.messagePublisher  = messagePublisher;
        }

        #endregion

        /// <summary>
        /// Process and delegate a <see cref="ClientChat"/> message from <see cref="IPlayer"/>.
        /// </summary>
        public void HandleClientChat(IPlayer player, ClientChat chat)
        {
            if (IsLocalChat(chat.Channel.ChatChannelId))
                HandleLocalChat(player, chat);
            else if (IsInternalChat(chat.Channel.ChatChannelId))
                HandleChatServerChat(player, chat);
            else
            {
                log.LogInformation($"ChatChannel {chat.Channel} has no handler implemented.");
                SendMessage(player.Session, "Currently not implemented", "GlobalChatManager", ChatChannelType.Debug);
            }
        }

        private void HandleLocalChat(IPlayer player, ClientChat chat)
        {
            ChatResult result = SendLocalChatMessage(player, chat);
            if (result != ChatResult.Ok)
            {
                player.Session.EnqueueMessageEncrypted(new ServerChatResult
                {
                    Channel    = chat.Channel,
                    ChatResult = result
                });
            }
            else
            {
                player.Session.EnqueueMessageEncrypted(new ServerChatAccept
                {
                    SenderName          = player.Name,
                    UnitId          = player.Guid,
                    GM            = player.Account.RbacManager.HasPermission(Permission.GMFlag),
                    ChatMessageId = chat.ChatMessageId
                });
            }  
        }

        private ChatResult SendLocalChatMessage(IPlayer player, ClientChat chat)
        {
            if (!textFilterManager.IsTextValid(chat.Message)
                || !textFilterManager.IsTextValid(chat.Message, UserText.Chat))
                return ChatResult.InvalidMessageText;

            var builder = new ChatMessageBuilder
            {
                Type     = chat.Channel.ChatChannelId,
                FromName = player.Name,
                Text     = chat.Message,
                Formats  = chatFormatManager.ToLocal(player, chat.Formats).ToList(),
                Guid     = player.Guid,
                GM       = player.Account.RbacManager.HasPermission(Permission.GMFlag)
            };

            player.Talk(builder, LocalChatDistance, player);
            return ChatResult.Ok;
        }

        private void HandleChatServerChat(IPlayer player, ClientChat chat)
        {
            messagePublisher.PublishAsync(new ChatChannelTextRequestMessage
            {
                Source        = player.Identity.ToInternalIdentity(),
                Type          = chat.Channel.ChatChannelId,
                ChatId        = chat.Channel.ChatId != 0 ? chat.Channel.ChatId : null,
                Text          = new Network.Internal.Message.Chat.Shared.ChatChannelText
                {
                    Text   = chat.Message,
                    Format = chatFormatManager.ToInternal(player, chat.Formats).ToList()
                },
                ChatMessageId = chat.ChatMessageId
            }).FireAndForgetAsync();
        }

        /// <summary>
        /// Handle's whisper messages between 2 clients
        /// </summary>
        public void HandleWhisperChat(IPlayer player, ClientChatWhisper whisper)
        {
            messagePublisher.PublishAsync(new ChatWhisperRequestMessage
            {
                Sender    = player.Identity.ToInternalIdentity(),
                Recipient = new Network.Internal.Message.Shared.IdentityName
                {
                    Name      = whisper.ToName,
                    RealmName = whisper.ToRealmName
                },
                Text = new Network.Internal.Message.Chat.Shared.ChatChannelText
                {
                    Text   = whisper.Message,
                    Format = chatFormatManager.ToInternal(player, whisper.Formats).ToList()
                },
                ChatMessageId    = whisper.ChatMessageId,
                IsAccountWhisper = whisper.IsAccountWhisper
            }).FireAndForgetAsync();
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
    }
}
