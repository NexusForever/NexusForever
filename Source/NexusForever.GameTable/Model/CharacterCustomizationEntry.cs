namespace NexusForever.GameTable.Model
{
    public class CharacterCustomizationEntry
    {
        public uint Id { get; set; }
        public uint RaceId { get; set; }
        public uint Gender { get; set; }
        public uint ItemSlotId { get; set; }
        public uint ItemDisplayId { get; set; }
        public uint Flags { get; set; }
        [GameTableFieldArray(2)]
        public uint[] CharacterCustomizationLabelId { get; set; }
        [GameTableFieldArray(2)]
        public uint[] Value { get; set; }
    }
}
