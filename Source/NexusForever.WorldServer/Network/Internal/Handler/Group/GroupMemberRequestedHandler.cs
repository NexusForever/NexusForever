using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Group
{
    public class GroupMemberRequestedHandler : IHandleMessages<GroupMemberRequestedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public GroupMemberRequestedHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(GroupMemberRequestedMessage message)
        {
            IPlayer player = playerManager.GetPlayer(message.Group.Leader.ToGameIdentity());
            if (player == null)
                return Task.CompletedTask;

            switch (message.Group.Request.Type)
            {
                case GroupRequestType.Request:
                {
                    player.Session.EnqueueMessageEncrypted(new ServerGroupRequestJoinResponse
                    {
                        GroupId    = message.Group.Id,
                        MemberInfo = new GroupMember
                        {
                            MemberIdentity = message.RequesterIdentity.ToNetworkIdentity(),
                            Member         = message.Requester.ToNetworkGroupCharacter()
                        }
                    });
                    break;
                }
                case GroupRequestType.Referral:
                {
                    player.Session.EnqueueMessageEncrypted(new ServerGroupReferral
                    {
                        GroupId         = message.Group.Id,
                        InviteeIdentity = message.RequesterIdentity.ToNetworkIdentity(),
                        InviteeName     = message.Requester.Name,
                    });
                    break;
                }
            }

            return Task.CompletedTask;
        }
    }
}
