using NexusForever.Database;
using NexusForever.Database.Character;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable.Model;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IItem : IDatabaseCharacter, IDatabaseState, INetworkBuildable<Item>
    {
        uint Id { get; }
        IItemInfo Info { get; }
        Spell4BaseEntry SpellEntry { get; }
        ulong Guid { get; }
        ulong? CharacterId { get; set; }
        InventoryLocation Location { get; set; }
        InventoryLocation PreviousLocation { get; set; }
        uint BagIndex { get; set; }
        uint PreviousBagIndex { get; set; }
        uint StackCount { get; set; }
        uint Charges { get; set; }
        float Durability { get; set; }
        uint ExpirationTimeLeft { get; set; }

        // <summary>
        /// Returns the <see cref="CurrencyType"/> this <see cref="IItem"/> sells for at a vendor.
        /// </summary>
        CurrencyType GetVendorSellCurrency(byte index);

        /// <summary>
        /// Returns the amount of <see cref="CurrencyType"/> this <see cref="IItem"/> sells for at a vendor.
        /// </summary>
        uint GetVendorSellAmount(byte index);
    }
}