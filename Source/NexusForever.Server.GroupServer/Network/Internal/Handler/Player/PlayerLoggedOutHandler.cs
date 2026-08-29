using NexusForever.Database.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Internal.Message.Player;
using NexusForever.Server.GroupServer.Character;
using NexusForever.Server.GroupServer.Group;
using Rebus.Handlers;

namespace NexusForever.Server.GroupServer.Network.Internal.Handler.Player
{
    public class PlayerLoggedOutHandler : IHandleMessages<PlayerLoggedOutMessage>
    {
        #region Dependency Injection

        private readonly CharacterManager _characterManager;
        private readonly GroupContext _context;

        public PlayerLoggedOutHandler(
            CharacterManager characterManager,
            GroupContext context)
        {
            _characterManager = characterManager;
            _context          = context;
        }

        #endregion

        public async Task Handle(PlayerLoggedOutMessage message)
        {
            Character.Character character = await _characterManager.GetCharacterAsync(message.Identity.ToGroupIdentity());
            if (character == null)
                return;

            foreach (CharacterGroup characterGroup in character.GetGroups())
            {
                Server.GroupServer.Group.Group group = await characterGroup.GetGroupAsync();
                if (group == null)
                    continue;

                GroupMember member = group.GetMember(character.Identity);
                if (member == null)
                    continue;

                await member.SetFlagAsync(GroupMemberInfoFlags.Disconnected);
            }

            await _context.SaveChangesAsync();
        }
    }
}
