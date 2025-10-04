using System.Linq;
using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Chat.Format;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Chat;
using NexusForever.Game.Static.Social;
using NexusForever.Network.Internal.Message.Chat;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Chat
{
    public class ChatWhisperTextHandler : IHandleMessages<ChatWhisperTextMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;
        private readonly IChatFormatManager chatFormatManager;

        public ChatWhisperTextHandler(
            IPlayerManager playerManager,
            IChatFormatManager chatFormatManager)
        {
            this.playerManager     = playerManager;
            this.chatFormatManager = chatFormatManager;
        }

        #endregion

        public Task Handle(ChatWhisperTextMessage message)
        {
            IPlayer player = playerManager.GetPlayer(message.Recipient.ToGameIdentity());
            if (player == null)
                return Task.CompletedTask;

            var builder = new ChatMessageBuilder();
            builder.Type    = message.IsAccountWhisper ? ChatChannelType.AccountWhisper : ChatChannelType.Whisper;
            builder.Text    = message.Text.Text;
            builder.Formats = chatFormatManager.ToNetwork(message.Text.Format).ToList();

            builder.FromName = message.SenderName.Name;
            if (message.Recipient.RealmId != message.Sender.RealmId)
                builder.FromRealm = message.SenderName.RealmName;

            player.Session.EnqueueMessageEncrypted(builder.Build());

            return Task.CompletedTask;
        }
    }
}
