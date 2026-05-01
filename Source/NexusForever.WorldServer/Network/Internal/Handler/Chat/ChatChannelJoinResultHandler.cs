using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Chat;
using NexusForever.Network.World.Message.Model.Chat;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Chat
{
    public class ChatChannelJoinResultHandler : IHandleMessages<ChatChannelJoinResultMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public ChatChannelJoinResultHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(ChatChannelJoinResultMessage message)
        {
            IPlayer player = playerManager.GetPlayer(message.Identity.ToGameIdentity());
            player?.Session.EnqueueMessageEncrypted(new ServerChatJoinResult
            {
                ChatChannelId = message.Type,
                Name   = message.Name,
                Result = message.Result
            });

            return Task.CompletedTask;
        }
    }
}
