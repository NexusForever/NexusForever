using NexusForever.Database.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Server.GroupServer.Character;
using Rebus.Handlers;

namespace NexusForever.Server.GroupServer.Network.Internal.Handler.Group
{
    public class GroupMarkerHandler : IHandleMessages<GroupMarkerMessage>
    {
        #region Dependency Injection

        private readonly GroupContext _context;
        private readonly IInternalMessagePublisher _messagePublisher;
        private readonly CharacterManager _characterManager;

        public GroupMarkerHandler(
            GroupContext context,
            OutboxMessagePublisher messagePublisher,
            CharacterManager characterManager)
        {
            _context          = context;
            _messagePublisher = messagePublisher;
            _characterManager = characterManager;
        }

        #endregion

        public async Task Handle(GroupMarkerMessage message)
        {
            (GroupActionResult? result, ulong groupId) = await SetTargetMarkerAsync(message);
            if (result != null)
            {
                await _messagePublisher.PublishAsync(new GroupActionResultMessage
                {
                    GroupId   = groupId,
                    Recipient = message.MarkerIndentity,
                    Target    = message.MarkerIndentity,
                    Result    = result.Value,
                });
            }

            await _context.SaveChangesAsync();
        }

        private async Task<(GroupActionResult?, ulong)> SetTargetMarkerAsync(GroupMarkerMessage message)
        {
            Character.Character character = await _characterManager.GetCharacterAsync(message.MarkerIndentity.ToGroupIdentity());
            if (character == null)
                return (GroupActionResult.NotInGroup, 0);

            // can only mark for active group.
            var group = await character.PrimaryGroup?.GetGroupAsync();
            if (group == null)
                return (GroupActionResult.NotInGroup, 0);

            return (await group.SetTargetMarkerAsync(message.MarkerIndentity.ToGroupIdentity(), message.GroupMarker, message.UnitId), group.Id);
        }
    }
}
