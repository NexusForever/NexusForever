using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Group;
using NexusForever.Game.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupSetLootRulesHandler : IMessageHandler<IWorldSession, ClientGroupSetLootRules>
    {
        /// <summary>
        /// </summary>
        public void HandleMessage(IWorldSession session, ClientGroupSetLootRules clientGroupSetLootRules)
        {
            IGroup group = GroupManager.Instance.GetGroupById(clientGroupSetLootRules.GroupId);
            if(group == null)
            {
                return;
            }

            IPlayer ruleChanger = session.Player;
            if(!ruleChanger.GroupMembershipForeground.IsPartyLeader)
            {
                return;
            }

            group.UpdateLootRules(clientGroupSetLootRules.LootRulesUnderThreshold, clientGroupSetLootRules.LootRulesThresholdAndOver, clientGroupSetLootRules.Threshold, clientGroupSetLootRules.HarvestingRule);
        }
    }
}