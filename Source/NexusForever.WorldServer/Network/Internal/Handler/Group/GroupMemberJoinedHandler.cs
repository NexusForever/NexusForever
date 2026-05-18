using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.Internal.Message.Group.Shared;
using NexusForever.Network.World.Message.Model;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Group
{
    public class GroupMemberJoinedHandler : IHandleMessages<GroupMemberJoinedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public GroupMemberJoinedHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(GroupMemberJoinedMessage message)
        {
            IPlayer player = playerManager.GetPlayer(message.AddedMember.Identity.ToGameIdentity());
            if (player == null)
                return Task.CompletedTask;

            player.Session.EnqueueMessageEncrypted(new ServerGroupJoin
            {
                Group        = message.Group.ToNetworkGroup(),
                TargetPlayer = message.AddedMember.Identity.ToNetworkIdentity()
            });

            foreach (GroupMember member in message.Group.Members)
            {
                if (message.AddedMember.Identity == member.Identity)
                    continue;

                ServerGroupMemberStatUpdate groupMemberStatUpdate = member.ToNetworkGroupMemberStatUpdate(message.Group.Id);
                player.Session.EnqueueMessageEncrypted(groupMemberStatUpdate);
            }

            return Task.CompletedTask;
        }
    }
}
