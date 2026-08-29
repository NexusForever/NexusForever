using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.Internal.Message.Group.Shared;
using NexusForever.Network.World.Message.Model;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Group
{
    public class GroupMemberRealmUpdatedHandler : IHandleMessages<GroupMemberRealmUpdatedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public GroupMemberRealmUpdatedHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(GroupMemberRealmUpdatedMessage message)
        {
            var groupUpdatePlayerRealm = new ServerGroupUpdatePlayerRealm
            {
                GroupId              = message.Group.Id,
                TargetPlayerIdentity = message.Member.Identity.ToNetworkIdentity(),
                RealmId              = message.Member.Character.RealmId,
                ZoneId               = message.Member.Character.WorldZoneId,
                MapId                = message.Member.Character.WorldId,
                PhaseId              = 1,
                IsSyncdToGroup       = true
            };

            foreach (GroupMember groupMember in message.Group.Members)
            {
                IPlayer player = playerManager.GetPlayer(groupMember.Identity.ToGameIdentity());
                player?.Session.EnqueueMessageEncrypted(groupUpdatePlayerRealm);
            }

            return Task.CompletedTask;
        }
    }
}
