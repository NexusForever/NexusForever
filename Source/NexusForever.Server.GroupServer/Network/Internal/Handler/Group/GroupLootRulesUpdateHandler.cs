using NexusForever.Database.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Server.GroupServer.Group;
using Rebus.Handlers;

namespace NexusForever.Server.GroupServer.Network.Internal.Handler.Group
{
    public class GroupLootRulesUpdateHandler : IHandleMessages<GroupLootRulesUpdateMessage>
    {
        #region Dependency Injection

        private readonly GroupContext _context;
        private readonly IInternalMessagePublisher _messagePublisher;
        private readonly GroupManager _groupManager;

        public GroupLootRulesUpdateHandler(
            GroupContext context,
            OutboxMessagePublisher messagePublisher,
            GroupManager groupManager)
        {
            _context          = context;
            _messagePublisher = messagePublisher;
            _groupManager     = groupManager;
        }

        #endregion

        public async Task Handle(GroupLootRulesUpdateMessage message)
        {
            GroupActionResult result = await SetLootRulesAsync(message);
            await _messagePublisher.PublishAsync(new GroupActionResultMessage
            {
                GroupId   = message.GroupId,
                Recipient = message.Identity,
                Target    = message.Identity,
                Result    = result,
            });

            await _context.SaveChangesAsync();
        }

        private async Task<GroupActionResult> SetLootRulesAsync(GroupLootRulesUpdateMessage message)
        {
            var group = await _groupManager.GetGroupAsync(message.GroupId);
            if (group == null)
                return GroupActionResult.InvalidGroup;

            return await group.SetLootRulesAsync(message.Identity.ToGroupIdentity(), message.NormalRule, message.ThresholdRule, message.ThresholdQuality, message.HarvestRule);
        }
    }
}
