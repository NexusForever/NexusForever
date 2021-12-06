using NexusForever.Game.Abstract.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupSetRoleHandler : IMessageHandler<IWorldSession, ClientGroupSetRole>
    {
        #region Dependency Injection

        private readonly IGroupManager groupManager;

        public ClientGroupSetRoleHandler(
            IGroupManager groupManager)
        {
            this.groupManager = groupManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientGroupSetRole groupSetRole)
        {
            GroupHelper.AssertGroupId(session, groupSetRole.GroupId);

            IGroup group = groupManager.GetGroupById(groupSetRole.GroupId);
            if (group == null)
            {
                GroupHelper.SendGroupResult(session, GroupResult.GroupNotFound, groupSetRole.GroupId, session.Player.Name);
                return;
            }

            group.UpdateMemberRole(session.Player.GroupMembership1, groupSetRole.TargetedPlayer, groupSetRole.ChangedFlag, groupSetRole.CurrentFlags.HasFlag(groupSetRole.ChangedFlag));
        }
    }
}
