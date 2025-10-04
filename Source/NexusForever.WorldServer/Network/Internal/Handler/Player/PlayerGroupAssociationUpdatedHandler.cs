using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Player;
using NexusForever.Network.World.Message.Model;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Player
{
    public class PlayerGroupAssociationUpdatedHandler : IHandleMessages<PlayerGroupAssociationUpdatedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public PlayerGroupAssociationUpdatedHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(PlayerGroupAssociationUpdatedMessage message)
        {
            IPlayer player = playerManager.GetPlayer(message.Identity.ToGameIdentity());
            if (player == null)
                return Task.CompletedTask;

            // TODO: Rawaho: not thread safe
            player.GroupAssociation = message.Group?.Id ?? 0;
            player.EnqueueToVisible(new ServerEntityGroupAssociation
            {
                UnitId  = player.Guid,
                GroupId = player.GroupAssociation
            }, true);

            return Task.CompletedTask;
        }
    }
}
