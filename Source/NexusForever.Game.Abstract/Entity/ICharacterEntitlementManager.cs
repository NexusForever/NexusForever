using NexusForever.Database.Character;
using NexusForever.Game.Abstract.Entitlement;

namespace NexusForever.Game.Abstract.Entity
{
    public interface ICharacterEntitlementManager : IEntitlementManager<ICharacterEntitlement>, IDatabaseCharacter
    {
    }
}
