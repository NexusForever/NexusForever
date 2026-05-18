using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Loot;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Loot
{
    public interface ILootInstance : IEnumerable<ILootInstanceItem>, IUpdate
    {
        uint Guid { get; }

        LootEntityType LootEntityType { get; }

        LooterType LooterType { get; }

        bool Explosion { get; set; }

        bool HasExpired { get; }

        void AddLootItem(uint staticId, LootItemType type, uint count);

        void SendLootNotify(IPlayer session);

        bool HasLootInstanceId(int lootInstanceId);

        bool HasLooter(ulong characterId);
    }
}
