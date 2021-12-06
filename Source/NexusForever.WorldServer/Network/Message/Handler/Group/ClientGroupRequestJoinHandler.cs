using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupRequestJoinHandler : IMessageHandler<IWorldSession, ClientGroupRequestJoin>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;
        private readonly IGroupManager groupManager;

        public ClientGroupRequestJoinHandler(
            IPlayerManager playerManager,
            IGroupManager groupManager)
        {
            this.playerManager = playerManager;
            this.groupManager  = groupManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientGroupRequestJoin joinRequest)
        {
            if (session.Player.GroupMembership1 != null) // player who did /join is already in a group. This has no effect.
                return;

            IPlayer targetedPlayer = playerManager.GetPlayer(joinRequest.Name);
            if (targetedPlayer == null)
                return;

            if (targetedPlayer.GroupMembership1 == null)
            {
                // Player and Target are not part of a group - create one for them both so /join acts as /invite.
                IGroup newGroup = groupManager.CreateGroup(session.Player);
                newGroup.Invite(session.Player, targetedPlayer);
            }
            else
            {
                IGroup group = targetedPlayer.GroupMembership1.Group;
                if (targetedPlayer.GroupMembership1.IsPartyLeader)  // /Join was on the leader - so just do a std Join request.
                    group.HandleJoinRequest(session.Player);
                else  //target player is not the leader of the group, so this acts as a referral
                    group.ReferMember(session.Player.GroupMembership1, targetedPlayer);
            }
        }
    }
}
