using NexusForever.Database.Group;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.Internal.Message.Player;
using NexusForever.Server.GroupServer.Character;
using Rebus.Handlers;

namespace NexusForever.Server.GroupServer.Network.Internal.Handler.Player
{
    public class PlayerStatUpdatedHandler : IHandleMessages<PlayerStatUpdatedMessage>
    {
        #region Dependency Injection

        private readonly GroupContext _groupContext;
        private readonly CharacterManager _characterManager;

        public PlayerStatUpdatedHandler(
            GroupContext groupContext,
            CharacterManager characterManager)
        {
            _groupContext     = groupContext;
            _characterManager = characterManager;
        }

        #endregion

        public async Task Handle(PlayerStatUpdatedMessage message)
        {
            if (!IsStatValid(message.Stat))
                return;

            var character = await _characterManager.GetCharacterAsync(message.Identity.ToGroupIdentity());
            if (character == null)
                return;

            character.SetStat(message.Stat, message.Value);

            await _groupContext.SaveChangesAsync();
        }

        private static bool IsStatValid(Stat stat)
        {
            return stat is Stat.Level
                or Stat.MentorLevel
                or Stat.Health
                or Stat.Shield
                or Stat.InterruptArmour
                or Stat.Focus;
        }
    }
}
