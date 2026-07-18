using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Account;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable.Model;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Loot
{
    public interface IGlobalLootManager : IUpdate
    {
        void Initialise();

        ILootInstance DropLoot(IPlayer looter, IWorldEntity lootedEntity);

        void DropLoot(IPlayer looter, IItem lootedItem);

        void DropLoot(uint groupId, IWorldEntity lootedEntity);

        void GiveLoot(IPlayer looter, int lootInstanceItemId);

        void GiveAllLootInRange(IPlayer looter);

        void GiveLoot(IPlayer looter, Item2Entry entry, uint count, uint lootUnitId);

        void GiveLoot(IPlayer looter, VirtualItemEntry entry, uint count, uint lootUnitId);

        void GiveLoot(IPlayer looter, AccountCurrencyType accountCurrencyType, uint count, uint lootUnitId);

        void GiveLoot(IPlayer looter, CurrencyType currencyType, uint count, uint lootUnitId);
    }
}
