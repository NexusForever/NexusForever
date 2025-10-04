using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Chat;
using NexusForever.Network.World.Message.Model;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Chat
{
    public class ChatChannelResultHandler : IHandleMessages<ChatChannelResultMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public ChatChannelResultHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(ChatChannelResultMessage message)
        {
            IPlayer player = playerManager.GetPlayer(message.Identity.ToGameIdentity());
            player?.Session.EnqueueMessageEncrypted(new ServerChatResult
            {
                Channel = new NexusForever.Network.World.Message.Model.Shared.Channel
                {
                    Type   = message.Type,
                    ChatId = message?.ChatId ?? 0,
                },
                ChatResult = message.Result
            });

            return Task.CompletedTask;
        }
    }
}
