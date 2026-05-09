using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Chat;
using NexusForever.Network.World.Message.Model.Chat;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Chat
{
    public class ChatWhisperFailedHandler : IHandleMessages<ChatWhisperFailedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public ChatWhisperFailedHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(ChatWhisperFailedMessage message)
        {
            IPlayer player = playerManager.GetPlayer(message.Sender.ToGameIdentity());
            player?.Session.EnqueueMessageEncrypted(new ServerChatWhisperFail
            {
                CharacterTo      = message.Recipient.Name,
                ChatMessageId    = message.ChatMessageId,
                IsAccountWhisper = message.IsAccountWhisper
            });

            return Task.CompletedTask;
        }
    }
}
