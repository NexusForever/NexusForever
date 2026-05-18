using NexusForever.Game.Static.Loot;

namespace NexusForever.Game.Abstract.Loot
{
    public interface ILootItem
    {
        LootItemType Type { get; }

        uint StaticId { get; }

        bool GetDrop(out uint count);
    }
}
