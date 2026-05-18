using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Loot;

namespace NexusForever.Game.Abstract.Loot
{
    public interface ILootGroup
    {
        ulong Id { get; }

        LootEntityType Type { get; }

        float Probability { get; }

        Dictionary<ILootItem, uint> GenerateLootDrops(IPlayer player);
    }
}
