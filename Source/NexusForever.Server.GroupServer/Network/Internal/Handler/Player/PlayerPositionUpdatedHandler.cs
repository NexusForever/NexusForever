using System.Numerics;
using NexusForever.Database.Group;
using NexusForever.Network.Internal.Message.Player;
using NexusForever.Server.GroupServer.Character;
using NexusForever.Server.GroupServer.Group;
using Rebus.Handlers;

namespace NexusForever.Server.GroupServer.Network.Internal.Handler.Player
{
    public class PlayerPositionUpdatedHandler : IHandleMessages<PlayerPositionUpdatedMessage>
    {
        #region Dependency Injection

        private readonly GroupContext _context;
        private readonly CharacterManager _characterManager;

        public PlayerPositionUpdatedHandler(
            GroupContext context,
            CharacterManager characterManager)
        {
            _context     = context;
            _characterManager = characterManager;
        }

        #endregion

        public async Task Handle(PlayerPositionUpdatedMessage message)
        {
            Character.Character character = await _characterManager.GetCharacterAsync(message.Identity.ToGroupIdentity());
            if (character == null)
                return;

            character.Position = new Vector3(message.Position.X, message.Position.Y, message.Position.Z);

            foreach (CharacterGroup characterGroup in character.GetGroups())
            {
                var group = await characterGroup.GetGroupAsync();
                if (group == null)
                    continue;

                GroupMember member = group.GetMember(character.Identity);
                if (member == null)
                    continue;

                member.PositionDirty = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}
