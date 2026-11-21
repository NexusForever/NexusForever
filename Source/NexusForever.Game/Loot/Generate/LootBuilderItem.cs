﻿using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Loot.Generate;
using NexusForever.Game.Static.Loot;

namespace NexusForever.Game.Loot.Generate
{
    public class LootBuilderItem : ILootBuilderItem
    {
        public LootItemType Type { get; private set; }
        public uint StaticId { get; private set; }
        public uint Count { get; private set; }
        public List<Identity> Looters { get; private set; } = [];

        /// <summary>
        /// Initialise <see cref="ILootBuilderItem"/>.
        /// </summary>
        /// <param name="type">The type of loot.</param>
        /// <param name="id">The identifier of the loot, specific to the specified <paramref name="type"/>.</param>
        /// <param name="count">The quantity of this loot item to drop.</param>
        /// <param name="looters">The player identities eligible to loot this item.</param>
        public void Initialise(LootItemType type, uint staticId, uint count, List<Identity> looters)
        {
            Type     = type;
            StaticId = staticId;
            Count    = count;
            Looters  = looters;
        }
    }
}
