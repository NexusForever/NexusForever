namespace NexusForever.GameTable.Model
{
    public class CharacterCustomizationEntry
    {
        public uint Id;
        public uint RaceId;
        public uint Gender;
        public uint ItemSlotId;
        public uint ItemDisplayId;
        public uint Flags;
        [GameTableFieldArray(2)]
        public uint[] CharacterCustomizationLabelId;
        [GameTableFieldArray(2)]
        public uint[] Value;
    }
}
