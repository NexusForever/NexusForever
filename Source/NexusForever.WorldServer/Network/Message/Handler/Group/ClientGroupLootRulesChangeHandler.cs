using NexusForever.Game;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared;

namespace NexusForever.WorldServer.Network.Message.Handler.Group
{
    public class ClientGroupLootRulesChangeHandler : IMessageHandler<IWorldSession, ClientGroupLootRulesChange>
    {
        #region Dependency Injection

        private readonly IInternalMessagePublisher messagePublisher;

        public ClientGroupLootRulesChangeHandler(
            IInternalMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientGroupLootRulesChange groupLootRulesChange)
        {
            messagePublisher.PublishAsync(new GroupLootRulesUpdateMessage
            {
                GroupId          = groupLootRulesChange.GroupId,
                Identity         = session.Player.Identity.ToInternalIdentity(),
                NormalRule       = groupLootRulesChange.LootRulesUnderThreshold,
                ThresholdRule    = groupLootRulesChange.LootRulesThresholdAndOver,
                ThresholdQuality = groupLootRulesChange.Threshold,
                HarvestRule      = groupLootRulesChange.HarvestingRule
            }).FireAndForgetAsync();
        }
    }
}
