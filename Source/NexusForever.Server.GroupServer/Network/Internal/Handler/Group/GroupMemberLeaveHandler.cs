using NexusForever.Database.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Server.GroupServer.Group;
using Rebus.Handlers;

namespace NexusForever.Server.GroupServer.Network.Internal.Handler.Group
{
    public class GroupMemberLeaveHandler : IHandleMessages<GroupMemberLeaveMessage>
    {
        #region Dependency Injection

        private readonly GroupContext _context;
        private readonly IInternalMessagePublisher _messagePublisher;
        private readonly GroupManager _groupManager;

        public GroupMemberLeaveHandler(
            GroupContext context,
            OutboxMessagePublisher messagePublisher,
            GroupManager groupManager)
        {
            _context          = context;
            _messagePublisher = messagePublisher;
            _groupManager     = groupManager;
        }

        #endregion

        public async Task Handle(GroupMemberLeaveMessage message)
        {
            GroupActionResult result = await HandleRemoveMemberAsync(message);
            await _messagePublisher.PublishAsync(new GroupActionResultMessage
            {
                GroupId   = message.GroupId,
                Recipient = message.Identity,
                Target    = message.Identity,
                Result    = result,
            });

            await _context.SaveChangesAsync();
        }

        private async Task<GroupActionResult> HandleRemoveMemberAsync(GroupMemberLeaveMessage message)
        {
            var group = await _groupManager.GetGroupAsync(message.GroupId);
            if (group == null)
                return GroupActionResult.InvalidGroup;

            return await group.RemoveMemberAsync(message.Identity.ToGroupIdentity(), RemoveReason.Left);
        }
    }
}
