using NexusForever.Game.Abstract.Group;
using NexusForever.Game.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupSetMemberFlagsHandler : IMessageHandler<IWorldSession, ClientGroupSetMemberFlags>
    {
        /// <summary>
        /// </summary>
        public void HandleMessage(IWorldSession session, ClientGroupSetMemberFlags clientSetMemberFlags)
        {
            GroupHelper.AssertGroupId(session, clientSetMemberFlags.GroupId);

            IGroup group = GroupManager.Instance.GetGroupById(clientSetMemberFlags.GroupId);
            if (group == null)
            {
                GroupHelper.SendGroupResult(session, GroupResult.GroupNotFound, clientSetMemberFlags.GroupId, session.Player.Name);
                return;
            }

            group.UpdateMemberRole(session.Player.GroupMembershipForeground, clientSetMemberFlags.TargetedPlayer, clientSetMemberFlags.ChangedFlag, clientSetMemberFlags.CurrentFlags.HasFlag(clientSetMemberFlags.ChangedFlag));
        }
    }
}