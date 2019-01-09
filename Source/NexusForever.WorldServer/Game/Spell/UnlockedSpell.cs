using System;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;
using ItemEntity = NexusForever.WorldServer.Game.Entity.Item;

namespace NexusForever.WorldServer.Game.Spell
{
    public class UnlockedSpell : ISaveCharacter
    {
        public Spell4BaseEntry Entry { get; }
        public byte Tier { get; set; }
        public ItemEntity Item { get; }

        /// <summary>
        /// Create a new <see cref="UnlockedSpell"/> from a <see cref="Spell4BaseEntry"/> entry.
        /// </summary>
        public UnlockedSpell(Spell4BaseEntry entry, byte tier, ItemEntity item)
        {
            Entry    = entry ?? throw new ArgumentNullException();
            Tier     = tier;
            Item     = item;
        }

        public void Save(CharacterContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
