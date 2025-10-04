using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Chat;
using NexusForever.Network.World.Message.Model;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Chat
{
    public class ChatChannelTextResultHandler : IHandleMessages<ChatChannelTextResultMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public ChatChannelTextResultHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(ChatChannelTextResultMessage message)
        {
            IPlayer player = playerManager.GetPlayer(message.Identity.ToGameIdentity());
            player?.Session.EnqueueMessageEncrypted(new ServerChatResult
            {
                Channel = new NexusForever.Network.World.Message.Model.Shared.Channel
                {
                    Type   = message.Type,
                    ChatId = message?.ChatId ?? 0,
                },
                ChatResult    = message.Result,
                ChatMessageId = message.ChatMessageId
            });

            return Task.CompletedTask;
        }
    }
}
