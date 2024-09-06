using NexusForever.Game.Abstract.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    internal class ClientGroupMarkUnitHandler : IMessageHandler<IWorldSession, ClientGroupMark>
    {
        #region Dependency Injection

        private readonly IGroupManager groupManager;

        public ClientGroupMarkUnitHandler(
            IGroupManager groupManager)
        {
            this.groupManager = groupManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientGroupMark groupMark)
        {
            // Players can only mark for their Active group.
            ulong groupId = session.Player.GroupMembership1.Group.Id;
            IGroup group = groupManager.GetGroupById(groupId);
            if (group == null)
            {
                GroupHelper.SendGroupResult(session, GroupResult.GroupNotFound, groupId, session.Player.Name);
                return;
            }

            GroupHelper.AssertPermission(session, groupId, GroupMemberInfoFlags.CanMark);
            group.MarkUnit(groupMark.UnitId, groupMark.Marker);
        }
    }
}
