using NexusForever.Game.Abstract.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupKickHandler : IMessageHandler<IWorldSession, ClientGroupKick>
    {
        #region Dependency Injection

        private readonly IGroupManager groupManager;

        public ClientGroupKickHandler(
            IGroupManager groupManager)
        {
            this.groupManager = groupManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientGroupKick groupKick)
        {
            GroupHelper.AssertGroupId(session, groupKick.GroupId);
            GroupHelper.AssertPermission(session, groupKick.GroupId, GroupMemberInfoFlags.CanKick);

            IGroup group = groupManager.GetGroupById(groupKick.GroupId);
            if (group == null)
            {
                GroupHelper.SendGroupResult(session, GroupResult.GroupNotFound, groupKick.GroupId, session.Player.Name);
                return;
            }

            // I never want to leave a group with only 1 member; So as with the Leave if there would be 1 member left after this operation
            // Just .Disband() the group.
            // TODO: If WoW is anything to go by; instance groups do NOT disband like this; once the instance is closed the group will be cleaned up.
            if (group.MemberCount == 2 && group.IsOpenWorld)
                group.Disband();
            else
                group.KickMember(groupKick.TargetedPlayer);
        }
    }
}
