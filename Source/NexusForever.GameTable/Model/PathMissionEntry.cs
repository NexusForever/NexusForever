namespace NexusForever.GameTable.Model
{
    public class PathMissionEntry
    {
        public uint Id { get; set; }
        public uint Creature2IdUnlock { get; set; }
        public uint PathTypeEnum { get; set; }
        public uint PathMissionTypeEnum { get; set; }
        public uint PathMissionDisplayTypeEnum { get; set; }
        public uint ObjectId { get; set; }
        public uint LocalizedTextIdName { get; set; }
        public uint LocalizedTextIdSummary { get; set; }
        public uint PathEpisodeId { get; set; }
        public uint WorldLocation2Id00 { get; set; }
        public uint WorldLocation2Id01 { get; set; }
        public uint WorldLocation2Id02 { get; set; }
        public uint WorldLocation2Id03 { get; set; }
        public uint PathMissionFlags { get; set; }
        public uint PathMissionFactionEnum { get; set; }
        public uint PrerequisiteId { get; set; }
        public uint LocalizedTextIdCommunicator { get; set; }
        public uint LocalizedTextIdUnlock { get; set; }
        public uint LocalizedTextIdSoldierOrders { get; set; }
        public uint Creature2IdContactOverride { get; set; }
        public uint QuestDirectionId { get; set; }
    }
}
