using NexusForever.Database.Group;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.Internal.Message.Player;
using NexusForever.Server.GroupServer.Character;
using Rebus.Handlers;

namespace NexusForever.Server.GroupServer.Network.Internal.Handler.Player
{
    public class PlayerPropertyUpdatedHandler : IHandleMessages<PlayerPropertyUpdatedMessage>
    {
        #region Dependency Injection

        private readonly GroupContext _groupContext;
        private readonly CharacterManager _characterManager;

        public PlayerPropertyUpdatedHandler(
            GroupContext groupContext,
            CharacterManager characterManager)
        {
            _groupContext     = groupContext;
            _characterManager = characterManager;
        }

        #endregion

        public async Task Handle(PlayerPropertyUpdatedMessage message)
        {
            if (!IsPropertyValid(message.Property))
                return;

            var character = await _characterManager.GetCharacterAsync(message.Identity.ToGroupIdentity());
            if (character == null)
                return;

            character.SetProperty(message.Property, message.Value);

            await _groupContext.SaveChangesAsync();
        }

        private static bool IsPropertyValid(Property property)
        {
            return property is Property.BaseHealth
                or Property.ShieldCapacityMax
                or Property.InterruptArmorThreshold
                or Property.BaseFocusPool;
        }
    }
}
