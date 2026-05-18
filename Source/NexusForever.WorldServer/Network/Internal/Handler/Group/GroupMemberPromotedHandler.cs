using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.Internal.Message.Group.Shared;
using NexusForever.Network.World.Message.Model;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Group
{
    public class GroupMemberPromotedHandler : IHandleMessages<GroupMemberPromotedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public GroupMemberPromotedHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(GroupMemberPromotedMessage message)
        {
            var groupPromote = new ServerGroupPromote
            {
                GroupId     = message.Group.Id,
                LeaderIndex = message.Member.GroupIndex,
                NewLeader   = message.Member.Identity.ToNetworkIdentity()
            };

            foreach (GroupMember member in message.Group.Members)
            {
                IPlayer player = playerManager.GetPlayer(member.Identity.ToGameIdentity());
                player?.Session.EnqueueMessageEncrypted(groupPromote);
            }

            return Task.CompletedTask;
        }
    }
}
