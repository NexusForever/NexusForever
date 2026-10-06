using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Loot;
using NexusForever.Network.Message;
using NetworkLootItem = NexusForever.Network.World.Message.Model.Loot.LootItem;

namespace NexusForever.Game.Abstract.Loot
{
    public interface ILootInstanceItem : INetworkBuildable<NetworkLootItem>
    {
        int Id { get; }

        uint Guid { get; }

        uint StaticId { get; }

        LootItemType Type { get; }

        uint Amount { get; }

        uint WinnerGuid { get; }

        ulong WinnerCharacterId { get; }

        bool Delivered { get; }

        void AddToAmount(uint amount);

        void SetWinner(ulong characterId, uint guid);

        void SetLootUnit(uint guid);

        void DeliverItem(IPlayer player, bool sendAsGrant = true);

        IEnumerable<NetworkLootItem> BuildForAccountCurrency();

        void DeliverItemOffline();
    }
}
