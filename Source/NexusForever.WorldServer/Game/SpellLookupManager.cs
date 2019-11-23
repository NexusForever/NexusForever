using NexusForever.Shared;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.WorldServer.Game
{
    public class SpellLookupManager : Singleton<SpellLookupManager>
    {
        private Dictionary<DashDirection, SpellGeneric> dashDirections = new Dictionary<DashDirection, SpellGeneric>
        {
            { DashDirection.Left, SpellGeneric.DashLeft },
            { DashDirection.Right, SpellGeneric.DashRight },
            { DashDirection.Forward, SpellGeneric.DashForward },
            { DashDirection.Backward, SpellGeneric.DashBackward }
        };

        private SpellLookupManager()
        {
        }

        public void Initialise()
        {
        }

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
