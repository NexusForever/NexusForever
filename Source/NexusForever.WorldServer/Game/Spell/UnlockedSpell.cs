using System;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;
using ItemEntity = NexusForever.WorldServer.Game.Entity.Item;

namespace NexusForever.WorldServer.Game.Spell
{
    public class UnlockedSpell : ISaveCharacter
    {
        public SpellBaseInfo Info { get; }
        public byte Tier { get; set; }
        public ItemEntity Item { get; }

        /// <summary>
        /// Create a new <see cref="UnlockedSpell"/> from a <see cref="SpellBaseInfo"/>.
        /// </summary>
        public UnlockedSpell(SpellBaseInfo info, byte tier, ItemEntity item)
        {
            Info = info ?? throw new ArgumentNullException();
            Tier = tier;
            Item = item;
        }

        public void Save(CharacterContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
