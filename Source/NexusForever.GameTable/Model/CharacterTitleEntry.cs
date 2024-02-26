namespace NexusForever.GameTable.Model
{
    public class CharacterTitleEntry
    {
        public uint Id { get; set; }
        public uint CharacterTitleCategoryId { get; set; }
        public uint LocalizedTextIdName { get; set; }
        public uint LocalizedTextIdTitle { get; set; }
        public uint Spell4IdActivate { get; set; }
        public uint LifeTimeSeconds { get; set; }
        public uint PlayerTitleFlagsEnum { get; set; }
        public uint ScheduleId { get; set; }
    }
}
