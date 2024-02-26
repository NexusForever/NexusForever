namespace NexusForever.GameTable.Model
{
    public class PetFlairEntry
    {
        public uint Id { get; set; }
        [GameTableFieldArray(2)]
        public uint[] UnlockBitIndex { get; set; }
        public uint Type { get; set; }
        public uint Spell4Id { get; set; }
        public uint LocalizedTextIdTooltip { get; set; }
        [GameTableFieldArray(2)]
        public uint[] ItemDisplayId { get; set; }
        public uint PrerequisiteId { get; set; }
    }
}
