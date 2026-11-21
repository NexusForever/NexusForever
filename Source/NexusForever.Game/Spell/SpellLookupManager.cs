using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Static;
using NexusForever.Game.Static.Entity.Movement;
using NexusForever.Shared;

namespace NexusForever.Game.Spell
{
    public class SpellLookupManager : Singleton<SpellLookupManager>, ISpellLookupManager
    {
        private Dictionary<DashDirection, SpellGeneric> dashDirections = new Dictionary<DashDirection, SpellGeneric>
        {
            { DashDirection.Left, SpellGeneric.DashLeft },
            { DashDirection.Right, SpellGeneric.DashRight },
            { DashDirection.Forward, SpellGeneric.DashForward },
            { DashDirection.Back, SpellGeneric.DashBackward }
        };

        /// <summary>
        /// Returns the Spell4 ID for the provided <see cref="DashDirection"/>.
        /// </summary>
        public bool TryGetDashSpell(DashDirection direction, out uint dashSpellId)
        {
            dashSpellId = 0;

            if (!dashDirections.TryGetValue(direction, out SpellGeneric dashSpell))
                return false;

            dashSpellId = (uint)dashSpell;
            return true;
        }
    }
}
