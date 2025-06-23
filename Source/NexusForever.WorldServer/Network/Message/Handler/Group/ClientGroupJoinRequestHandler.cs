using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Group;
using NexusForever.Game.Entity;
using NexusForever.Game.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupJoinRequestHandler : IMessageHandler<IWorldSession, ClientGroupJoinRequest>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;
        private readonly IGroupManager groupManager;

        public ClientGroupJoinRequestHandler(
            IPlayerManager playerManager,
            IGroupManager groupManager)
        {
            this.playerManager = playerManager;
            this.groupManager  = groupManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientGroupJoinRequest joinRequest)
        {
            IPlayer joinRequester = session.Player;
            IPlayer targetedPlayer = PlayerManager.Instance.GetPlayer(joinRequest.GroupMemberName);

            if (joinRequester.GroupMembershipForeground != null) // player who did /join is already in a group. This has no effect.
                return;

            if (targetedPlayer == null)
                return;

            if (targetedPlayer.GroupMembershipForeground == null)
            {
                // Player and Target are not part of a group - create one for them both so /join acts as /invite.
                IGroup newGroup = GroupManager.Instance.CreateGroup(joinRequester);
                newGroup.Invite(session.Player, targetedPlayer);
            }
            else
            {
                IGroup group = targetedPlayer.GroupMembershipForeground.Group;
                if (targetedPlayer.GroupMembershipForeground.IsPartyLeader)  // /Join was on the leader - so just do a std Join request.
                    group.HandleJoinRequest(session.Player);
                else  //target player is not the leader of the group, so this acts as a referral
                    group.ReferMember(session.Player.GroupMembershipForeground, targetedPlayer);
            }
        }
    }
}
