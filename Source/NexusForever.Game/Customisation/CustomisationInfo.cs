using NexusForever.Game.Abstract.Customisation;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Customisation
{
    public class CustomisationInfo : ICustomisationInfo
    {
        public Race Race { get; private set; }
        public Sex Sex { get; private set; }
        public uint[] Label { get; private set; }
        public uint[] Value { get; private set; }
        public ItemSlot Slot { get; private set; }
        public uint DisplayId { get; private set; }

        /// <summary>
        /// Create a new <see cref="ICustomisationInfo"/> from <see cref="CharacterCustomizationEntry"/>.
        /// </summary>
        public CustomisationInfo(CharacterCustomizationEntry entry)
        {
            Race      = (Race)entry.RaceId;
            Sex       = (Sex)entry.Gender;
            Label     = entry.CharacterCustomizationLabelId;
            Value     = entry.Value;
            Slot      = (ItemSlot)entry.ItemSlotId;
            DisplayId = entry.ItemDisplayId;
        }

        /// <summary>
        /// Create a new <see cref="ICustomisationInfo"/> from supplied data.
        /// </summary>
        public CustomisationInfo(Race race, Sex sex, uint[] label, uint[] value, ItemSlot slot, uint displayId)
        {
            Race      = race;
            Sex       = sex;
            Label     = label;
            Value     = value;
            Slot      = slot;
            DisplayId = displayId;
        }
    }
}
