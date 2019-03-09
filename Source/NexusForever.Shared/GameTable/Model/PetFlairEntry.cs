namespace NexusForever.Shared.GameTable.Model
{
    public class PetFlairEntry
    {
        public uint Id;
        [GameTableFieldArray(2)]
        public uint[] UnlockBitIndex;
        public uint Type;
        public uint Spell4Id;
        public uint LocalizedTextIdTooltip;
        [GameTableFieldArray(2)]
        public uint[] ItemDisplayId;
        public uint PrerequisiteId;
    }
}
