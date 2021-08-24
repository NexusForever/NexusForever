using NexusForever.Database.World.Model;
using NexusForever.WorldServer.Game.Loot.Static;
using System;

namespace NexusForever.WorldServer.Game.Loot
{
    public class LootItem
    {
        public LootItemType Type { get; }
        public uint StaticId { get; }

        private float probability;
        private uint minCount;
        private uint maxCount;

        /// <summary>
        /// Instatiate a <see cref="LootItem"/> with a database model.
        /// </summary>
        /// <param name="model"></param>
        public LootItem(LootItemModel model)
        {
            Type = (LootItemType)model.Type;
            StaticId = model.StaticId;
            probability = model.Probability;
            minCount = model.MinCount;
            maxCount = model.MaxCount;
        }

        /// <summary>
        /// Check this <see cref="LootItem"/> to get a random chance of it dropping.
        /// </summary>
        public bool GetDrop(out uint count)
        {
            count = 0u;

            double chance = new Random().NextDouble() * 100d;
            if (chance < probability)
            {
                count = (uint)new Random().Next((int)minCount, (int)(maxCount + 1));
                return true;
            }

            return false;
        }
    }
}
