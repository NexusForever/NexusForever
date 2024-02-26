namespace NexusForever.GameTable.Model
{
    public class PathRewardEntry
    {
        public uint Id { get; set; }
        public uint PathRewardTypeEnum { get; set; }
        public uint ObjectId { get; set; }
        public uint Spell4Id { get; set; }
        public uint Item2Id { get; set; }
        public uint Quest2Id { get; set; }
        public uint CharacterTitleId { get; set; }
        public uint PrerequisiteId { get; set; }
        public uint Count { get; set; }
        public uint PathRewardFlags { get; set; }
        public uint PathScientistScanBotProfileId { get; set; }
    }
}
