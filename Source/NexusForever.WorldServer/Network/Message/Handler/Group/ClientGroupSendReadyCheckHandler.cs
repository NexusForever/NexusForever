using NexusForever.Game.Abstract.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupSendReadyCheckHandler : IMessageHandler<IWorldSession, ClientGroupSendReadyCheck>
    {
        #region Dependency Injection

        private readonly IGroupManager groupManager;

        public ClientGroupSendReadyCheckHandler(
            IGroupManager groupManager)
        {
            this.groupManager = groupManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientGroupSendReadyCheck groupSendReadyCheck)
        {
            GroupHelper.AssertGroupId(session, groupSendReadyCheck.GroupId);

            IGroup group = groupManager.GetGroupById(groupSendReadyCheck.GroupId);
            if (group == null)
            {
                GroupHelper.SendGroupResult(session, GroupResult.GroupNotFound, groupSendReadyCheck.GroupId, session.Player.Name);
                return;
            }

            if (group.IsRaid && !session.Player.GroupMembership1.IsPartyLeader)
                GroupHelper.AssertPermission(session, group.Id, GroupMemberInfoFlags.CanReadyCheck);
            else
                GroupHelper.AssertGroupLeader(session, group.Id);

            group.PrepareForReadyCheck();
            group.PerformReadyCheck(session.Player, groupSendReadyCheck.Message);
        }
    }
}
