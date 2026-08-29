using System.Linq;
using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.World.Message.Model;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Group
{
    public class GroupPlayerInvitedHandler : IHandleMessages<GroupPlayerInvitedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public GroupPlayerInvitedHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(GroupPlayerInvitedMessage message)
        {
            IPlayer player = playerManager.GetPlayer(message.InviteeIdentity.ToGameIdentity());
            player?.Session.EnqueueMessageEncrypted(new ServerGroupInviteReceived
            {
                GroupId      = message.Group.Id,
                LeaderIndex  = message.Leader.GroupIndex - 1,
                InviterIndex = message.Inviter.GroupIndex - 1,
                Members      = message.Group.Members.Select(m => m.Character.ToNetworkGroupCharacter()).ToList(),
            });

            return Task.CompletedTask;
        }
    }
}
