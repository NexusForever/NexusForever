using System;
using System.Collections;
using System.Collections.Generic;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Spell.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Game.Spell
{
    public class ActionSet : ISaveCharacter, IEnumerable<ActionSetAction>
    {
        public const byte MaxTierPoints = 42;
        public const byte MaxActionSets = 4;
        public const byte MaxActionCount = 48;

        public byte Index { get; }
        public byte TierPoints { get; set; }

        private readonly List<ActionSetAction> actions = new List<ActionSetAction>();

        /// <summary>
        /// Create a new <see cref="ActionSet"/> with supplied index.
        /// </summary>
        public ActionSet(byte index)
        {
            Index      = index;
            TierPoints = MaxTierPoints;
        }

        /// <summary>
        /// Add <see cref="UnlockedSpell"/> to supplied <see cref="UILocation"/>.
        /// </summary>
        public void AddSpell(UnlockedSpell spell, UILocation location, byte tier)
        {
            byte tierPointCost = 0;
            for (byte i = spell.Tier; i <= tier; i++)
                tierPointCost += (byte)(i == 5 || i == 9 ? 5 : 1);

            TierPoints -= tierPointCost;

            if (tierPointCost > TierPoints)
                throw new InvalidOperationException();

            actions.Add(new ActionSetAction(4, spell.Info.Entry.Id, location));
        }

        public void Save(CharacterContext context)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<ActionSetAction> GetEnumerator()
        {
            return actions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
