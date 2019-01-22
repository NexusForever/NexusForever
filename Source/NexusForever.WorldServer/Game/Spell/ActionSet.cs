using System;
using System.Linq;
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
        public const byte MaxAMPPoints = 45 + 10; // 10 are bonus unlocked
        public const byte MaxTier = 9;

        public byte Index { get; }
        public byte TierPoints { get; private set; }
        public byte AMPPoints { get; private set; }

        public List<ActionSetAction> actions { get; } = new List<ActionSetAction>();
        public List<ushort> AMPs { get; } = new List<ushort>();

        /// <summary>
        /// Create a new <see cref="ActionSet"/> with supplied index.
        /// </summary>
        public ActionSet(byte index)
        {
            Index      = index;
            TierPoints = MaxTierPoints;
            AMPPoints  = MaxAMPPoints -10;
        }

        /// <summary>
        /// Add <see cref="UnlockedSpell"/> to supplied <see cref="UILocation"/>.
        /// </summary>
        public void AddSpell(UnlockedSpell spell, UILocation location, byte tier)
        {
            TierPoints -= TierPointCost(tier);

            if (TierPoints < 0 || TierPoints > MaxTierPoints)
                throw new InvalidOperationException();

            actions.Add(new ActionSetAction(4, spell.Info.Entry.Id, location, tier));
        }

        public void UpdateSpellTier(UnlockedSpell spell, byte tier)
        {
            var itemToUpdate = actions.FirstOrDefault(a => a.ObjectId == spell.Info.Entry.Id);
            if (itemToUpdate == null)
                return;

            TierPoints += TierPointCost(itemToUpdate.Tier);
            TierPoints -= TierPointCost(tier);

            if (TierPoints < 0 || TierPoints > MaxTierPoints)
                throw new InvalidOperationException();

            itemToUpdate.Tier = tier;
        }

        public void RemoveSpell(UILocation location)
        {
            var itemToRemove = actions.FirstOrDefault(a => a.Location == location);
            if (itemToRemove == null)
                return;

            TierPoints += TierPointCost(itemToRemove.Tier);

            if (TierPoints < 0 || TierPoints > MaxTierPoints)
                throw new InvalidOperationException();

            actions.Remove(itemToRemove);
        }

        public byte TierPointCost(byte tier)
        {
            if (tier > MaxTier)
                throw new InvalidOperationException();
                
            byte tierPointCost = 0;
            for (byte i = 2; i <= tier; i++)
                tierPointCost += (byte)(i == 5 || i == 9 ? 5 : 1);

            return tierPointCost;
        }
        
        public ActionSetAction GetSpell(UILocation location)
        { 
            return actions.FirstOrDefault(a => a.Location == location);
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
