using System.Threading.Tasks;
using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.Internal.Message.Group.Shared;
using NexusForever.Network.World.Message.Model;
using Rebus.Handlers;

namespace NexusForever.WorldServer.Network.Internal.Handler.Group
{
    public class GroupLootRulesUpdatedHandler : IHandleMessages<GroupLootRulesUpdatedMessage>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public GroupLootRulesUpdatedHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public Task Handle(GroupLootRulesUpdatedMessage message)
        {
            var groupLootRulesChanged = new ServerGroupLootRulesChange
            {
                GroupId                   = message.Group.Id,
                LootRulesUnderThreshold   = message.Group.NormalRule,
                LootRulesThresholdAndOver = message.Group.ThresholdRule,
                LootThreshold             = message.Group.ThresholdQuality,
                HarvestLootRule           = message.Group.HarvestRule
            };

            foreach (GroupMember groupMember in message.Group.Members)
            {
                IPlayer player = playerManager.GetPlayer(groupMember.Identity.ToGameIdentity());
                player?.Session.EnqueueMessageEncrypted(groupLootRulesChanged);
            }

            return Task.CompletedTask;
        }
    }
}
