using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.Internal.Message.Group.Shared;
using NexusForever.Network.World.Message.Model;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Group
{
    public class GroupMarkerUpdatedHandler : IHandleMessages<GroupMarkerUpdatedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public GroupMarkerUpdatedHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(GroupMarkerUpdatedMessage message)
        {
            var groupMarkUnit = new ServerGroupMarkUnit
            {
                GroupId = message.Group.Id,
                Marker  = message.Marker,
                UnitId  = message.UnitId ?? 0,
            };

            foreach (GroupMember groupMember in message.Group.Members)
            {
                IPlayer player = playerManager.GetPlayer(groupMember.Identity.ToGameIdentity());
                player?.Session.EnqueueMessageEncrypted(groupMarkUnit);
            }

            return Task.CompletedTask;
        }
    }
}
