using NexusForever.Database.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Server.GroupServer.Group;
using Rebus.Handlers;

namespace NexusForever.Server.GroupServer.Network.Internal.Handler.Group
{
    public class GroupMemberKickHandler : IHandleMessages<GroupMemberKickMessage>
    {
        #region Dependency Injection

        private readonly GroupContext _context;
        private readonly IInternalMessagePublisher _messagePublisher;
        private readonly GroupManager _groupManager;

        public GroupMemberKickHandler(
            GroupContext context,
            OutboxMessagePublisher messagePublisher,
            GroupManager groupManager)
        {
            _context          = context;
            _messagePublisher = messagePublisher;
            _groupManager     = groupManager;
        }

        #endregion

        public async Task Handle(GroupMemberKickMessage message)
        {
            GroupActionResult result = await HandleMemberKickAsync(message);
            await _messagePublisher.PublishAsync(new GroupActionResultMessage
            {
                GroupId   = message.GroupId,
                Recipient = message.Kicker,
                Target    = message.Kicked,
                Result    = result,
            });

            await _context.SaveChangesAsync();
        }

        private async Task<GroupActionResult> HandleMemberKickAsync(GroupMemberKickMessage message)
        {
            var group = await _groupManager.GetGroupAsync(message.GroupId);
            if (group == null)
                return GroupActionResult.InvalidGroup;

            return await group.KickMemberAsync(message.Kicker.ToGroupIdentity(), message.Kicked.ToGroupIdentity(), RemoveReason.Kicked);
        }
    }
}
