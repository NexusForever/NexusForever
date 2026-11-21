using NexusForever.Database.Character;
using NexusForever.Game.Static.Costume;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Abstract.Entity
{
    public interface ICostumeItem : IDatabaseCharacter
    {
        CostumeItemSlot Slot { get; }
        ItemSlot ItemSlot { get; }
        IItemInfo ItemInfo { get; }
        uint? Item2Id { get; set; }
        ushort? DisplayId { get; }
        uint DyeData { get; set; }

        /// <summary>
        /// Get <see cref="IItemVisual"/> for <see cref="ICostumeItem"/>.
        /// </summary>
        IItemVisual GetItemVisual();
    }
}