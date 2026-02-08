using NexusForever.Database.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Server.GroupServer.Group;
using Rebus.Handlers;

namespace NexusForever.Server.GroupServer.Network.Internal.Handler.Group
{
    public class GroupMemberFlagUpdateHandler : IHandleMessages<GroupMemberFlagUpdateMessage>
    {
        #region Dependency Injection

        private readonly GroupContext _context;
        private readonly IInternalMessagePublisher _messagePublisher;
        private readonly GroupManager _groupManager;

        public GroupMemberFlagUpdateHandler(
            GroupContext context,
            OutboxMessagePublisher messagePublisher,
            GroupManager groupManager)
        {
            _context          = context;
            _messagePublisher = messagePublisher;
            _groupManager     = groupManager;
        }

        #endregion

        public async Task Handle(GroupMemberFlagUpdateMessage message)
        {
            GroupActionResult result = await SetMemberFlagsAsync(message);
            await _messagePublisher.PublishAsync(new GroupActionResultMessage
            {
                GroupId   = message.GroupId,
                Recipient = message.Source,
                Target    = message.Target,
                Result    = result,
            });

            await _context.SaveChangesAsync();
        }

        private async Task<GroupActionResult> SetMemberFlagsAsync(GroupMemberFlagUpdateMessage message)
        {
            var group = await _groupManager.GetGroupAsync(message.GroupId);
            if (group == null)
                return GroupActionResult.InvalidGroup;

            return await group.SetMemberFlagsAsync(message.Source.ToGroupIdentity(), message.Target.ToGroupIdentity(), message.Flags);
        }
    }
}
