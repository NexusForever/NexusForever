using NexusForever.Game.Abstract.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupLeaveHandler : IMessageHandler<IWorldSession, ClientGroupLeave>
    {
        #region Dependency Injection

        private readonly IGroupManager groupManager;

        public ClientGroupLeaveHandler(
            IGroupManager groupManager)
        {
            this.groupManager = groupManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientGroupLeave groupLeave)
        {
            GroupHelper.AssertGroupId(session, groupLeave.GroupId);

            IGroup group = groupManager.GetGroupById(groupLeave.GroupId);
            if (group == null)
            {
                GroupHelper.SendGroupResult(session, GroupResult.GroupNotFound, groupLeave.GroupId, session.Player.Name);
                return;
            }

            // I never want to leave a group with only 1 member; So as with the Kick if there would be 1 member left after this operation
            // Just .Disband() the group.
            // TODO: If WoW is anything to go by; instance groups do NOT disband like this; once the instance is closed the group will be cleaned up.
            if (groupLeave.ShouldDisband || group.MemberCount == 2 && group.IsOpenWorld)
            {
                group.Disband();
                return;
            }

            //TODO: This may not be correct? I need to look into if i can leave my main group whilst part of an instance group.
            group.RemoveMember(session.Player.GroupMembership1);
        }
    }
}
