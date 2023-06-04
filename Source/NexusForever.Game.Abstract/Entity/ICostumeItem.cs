using NexusForever.Database.Character;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Abstract.Entity
{
    public interface ICostumeItem : IDatabaseCharacter
    {
        CostumeItemSlot Slot { get; }
        ItemSlot ItemSlot { get; }
        IItemInfo ItemInfo { get; }
        uint? ItemId { get; set; }
        ushort? DisplayId { get; }
        int DyeData { get; set; }

        /// <summary>
        /// Get <see cref="IItemVisual"/> for <see cref="ICostumeItem"/>.
        /// </summary>
        IItemVisual GetItemVisual();
    }
}