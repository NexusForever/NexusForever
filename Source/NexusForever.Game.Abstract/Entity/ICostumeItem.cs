using NexusForever.Database.Character;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Entity
{
    public interface ICostumeItem : IDatabaseCharacter
    {
        CostumeItemSlot Slot { get; }
        Item2Entry Entry { get; }
        uint ItemId { get; set; }
        int DyeData { get; set; }
    }
}