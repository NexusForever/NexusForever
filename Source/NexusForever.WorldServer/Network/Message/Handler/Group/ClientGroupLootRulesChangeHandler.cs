using NexusForever.Game.Abstract.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupLootRulesChangeHandler : IMessageHandler<IWorldSession, ClientGroupLootRulesChange>
    {
        #region Dependency Injection

        private readonly IGroupManager groupManager;

        public ClientGroupLootRulesChangeHandler(
            IGroupManager groupManager)
        {
            this.groupManager = groupManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientGroupLootRulesChange groupLootRulesChange)
        {
            GroupHelper.AssertGroupId(session, groupLootRulesChange.GroupId);
            GroupHelper.AssertGroupLeader(session, groupLootRulesChange.GroupId);

            IGroup group = groupManager.GetGroupById(groupLootRulesChange.GroupId);
            group.UpdateLootRules(groupLootRulesChange.LootRulesUnderThreshold, groupLootRulesChange.LootRulesThresholdAndOver, groupLootRulesChange.Threshold, groupLootRulesChange.HarvestingRule);
        }
    }
}
