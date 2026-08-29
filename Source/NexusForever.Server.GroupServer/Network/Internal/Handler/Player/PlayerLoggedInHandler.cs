using NexusForever.Database.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Network.Internal.Message.Player;
using NexusForever.Server.GroupServer.Character;
using NexusForever.Server.GroupServer.Group;
using Rebus.Handlers;

namespace NexusForever.Server.GroupServer.Network.Internal.Handler.Player
{
    public class PlayerLoggedInHandler : IHandleMessages<PlayerLoggedInMessage>
    {
        #region Dependency Injection

        private readonly CharacterManager _characterManager;
        private readonly IInternalMessagePublisher _messagePublisher;
        private readonly GroupContext _context;

        public PlayerLoggedInHandler(
            CharacterManager characterManager,
            OutboxMessagePublisher messagePublisher,
            GroupContext context)
        {
            _characterManager = characterManager;
            _messagePublisher = messagePublisher;
            _context          = context;
        }

        #endregion

        public async Task Handle(PlayerLoggedInMessage message)
        {
            Character.Character character = await _characterManager.GetCharacterAsync(message.Identity.ToGroupIdentity());
            if (character == null)
                return;

            // order by descending index so that the secondary group is processed first
            foreach (CharacterGroup characterGroup in character.GetGroups().OrderByDescending(g => g.Index))
            {
                var group = await characterGroup.GetGroupAsync();
                if (group == null)
                    continue;

                GroupMember member = group.GetMember(character.Identity);
                if (member == null)
                    continue;

                await member.RemoveFlagAsync(GroupMemberInfoFlags.Disconnected);

                await _messagePublisher.PublishAsync(new GroupMemberJoinedMessage
                {
                    AddedMember = await member.ToInternalGroupMember(),
                    Group       = await group.ToInternalGroup(),
                });

                if (characterGroup.Index == 0)
                {
                    await _messagePublisher.PublishAsync(new PlayerGroupAssociationUpdatedMessage
                    {
                        Identity = character.Identity.ToInternalIdentity(),
                        Group    = await group.ToInternalGroup(),
                    });
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
