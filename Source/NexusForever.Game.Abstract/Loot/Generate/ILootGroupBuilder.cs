using System.Collections.Immutable;
using NexusForever.Database.World.Model;

namespace NexusForever.Game.Abstract.Loot.Generate
{
    public interface ILootGroupBuilder
    {
        void Initialise(ImmutableList<LootGroupModel> lootGroups);

        IEnumerable<LootGroupModel> GetRootLootGroups();

        IEnumerable<LootGroupModel> GetLootGroupChildren(ulong id);
    }
}
