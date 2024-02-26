namespace NexusForever.GameTable.Model
{
    public class LevelUpUnlockEntry
    {
        public uint Id { get; set; }
        public uint LevelUpUnlockSystemEnum { get; set; }
        public uint Level { get; set; }
        public uint LevelUpUnlockTypeId { get; set; }
        public uint LocalizedTextIdDescription { get; set; }
        public string DisplayIcon { get; set; }
        public uint PrerequisiteId { get; set; }
        public uint LevelUpUnlockValue { get; set; }
    }
}
