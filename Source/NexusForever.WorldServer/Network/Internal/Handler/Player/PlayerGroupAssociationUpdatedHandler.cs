using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Group;
using NexusForever.Network.Internal.Message.Player;
using NexusForever.Network.World.Message.Model;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Player
{
    public class PlayerGroupAssociationUpdatedHandler : IHandleMessages<PlayerGroupAssociationUpdatedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;
        private readonly IGroupManager groupManager;

        public PlayerGroupAssociationUpdatedHandler(
            IPlayerManager playerManager,
            IGroupManager groupManager)
        {
            this.playerManager = playerManager;
            this.groupManager  = groupManager;
        }

        #endregion

        public async Task Handle(PlayerGroupAssociationUpdatedMessage message)
        {
            Identity identity = message.Identity.ToGameIdentity();
            if (message.Group != null)
                groupManager.AddGroupAssociation(message.Group, identity);
            else
                groupManager.RemoveGroupAssociation(identity);

            IPlayer player = playerManager.GetPlayer(identity);
            if (player == null)
                return;

            await player.SynchroniseAsync(() =>
            {
                player.GroupAssociation = message.Group?.Id ?? 0;
            });

            player.EnqueueToVisible(new ServerEntityGroupAssociation
            {
                UnitId  = player.Guid,
                GroupId = player.GroupAssociation
            }, true);
        }
    }
}
