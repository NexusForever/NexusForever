using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.Internal.Message.Group.Shared;
using NexusForever.Network.World.Message.Model;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Group
{
    public class GroupMemberStatsUpdatedHandler : IHandleMessages<GroupMemberStatsUpdatedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public GroupMemberStatsUpdatedHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(GroupMemberStatsUpdatedMessage message)
        {
            ServerGroupMemberStatUpdate groupMemberStatUpdate = message.Member.ToNetworkGroupMemberStatUpdate(message.Group.Id);

            foreach (GroupMember member in message.Group.Members)
            {
                if (message.Member.Identity == member.Identity)
                    continue;

                IPlayer player = playerManager.GetPlayer(member.Identity.ToGameIdentity());
                player?.Session.EnqueueMessageEncrypted(groupMemberStatUpdate);
            }

            return Task.CompletedTask;
        }
    }
}
