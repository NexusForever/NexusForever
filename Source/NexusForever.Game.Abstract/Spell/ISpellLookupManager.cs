using NexusForever.Game.Static.Entity.Movement;

namespace NexusForever.Game.Abstract.Spell
{
    public interface ISpellLookupManager
    {
        bool TryGetDashSpell(DashDirection direction, out uint dashSpellId);
    }
}