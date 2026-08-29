using NexusForever.Database.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Server.GroupServer.Group;
using Rebus.Handlers;

namespace NexusForever.Server.GroupServer.Network.Internal.Handler.Group
{
    public class GroupReadyCheckMessageHandler : IHandleMessages<GroupReadyCheckMessage>
    {
        #region Dependency Injection

        private readonly GroupContext _context;
        private readonly IInternalMessagePublisher _messagePublisher;
        private readonly GroupManager _groupManager;

        public GroupReadyCheckMessageHandler(
            GroupContext context,
            OutboxMessagePublisher messagePublisher,
            GroupManager groupManager)
        {
            _groupManager     = groupManager;
            _messagePublisher = messagePublisher;
            _context          = context;
        }

        #endregion

        public async Task Handle(GroupReadyCheckMessage message)
        {
            GroupActionResult? result = await StartReadyCheckAsync(message);
            if (result != null)
            {
                await _messagePublisher.PublishAsync(new GroupActionResultMessage
                {
                    GroupId   = message.GroupId,
                    Recipient = message.Initator,
                    Target    = message.Initator,
                    Result    = result.Value,
                });
            }

            await _context.SaveChangesAsync();
        }

        private async Task<GroupActionResult?> StartReadyCheckAsync(GroupReadyCheckMessage message)
        {
            var group = await _groupManager.GetGroupAsync(message.GroupId);
            if (group == null)
                return GroupActionResult.InvalidGroup;

            return await group.StartReadyCheckAsync(message.Initator.ToGroupIdentity(), message.Message);
        }
    }
}
