using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Group;
using NexusForever.Game.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupLeaveHandler : IMessageHandler<IWorldSession, ClientGroupLeave>
    {
        /// <summary>
        /// </summary>
        public void HandleMessage(IWorldSession session, ClientGroupLeave leave)
        {
            IPlayer leaver = session.Player;

            IGroup group = GroupManager.Instance.GetGroupById(leave.GroupId);
            if (group == null)
            {
                GroupHelper.SendGroupResult(session, GroupResult.GroupNotFound, leave.GroupId, leaver.Name);
                return;
            }

            // I never want to leave a group with only 1 member; So as with the Kick if there would be 1 member left after this operation
            // Just .Disband() the group.
            // TODO: If WoW is anything to go by; instance groups do NOT disband like this; once the instance is closed the group will be cleaned up.
            if (leave.Disband || group.MemberCount == 2 && group.IsOpenWorld)
            {
                group.Disband();
                return;
            }

            //TODO: This may not be correct? I need to look into if i can leave my main group whilst part of an instance group.
            group.RemoveMember(leaver.GroupMembershipForeground);
        }
    }
}