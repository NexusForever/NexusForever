namespace NexusForever.GameTable.Model
{
    public class PublicEventObjectiveEntry
    {
        public uint Id { get; set; }
        public uint PublicEventId { get; set; }
        public uint PublicEventObjectiveFlags { get; set; }
        public uint PublicEventObjectiveTypeSpecificFlags { get; set; }
        public uint WorldLocation2Id { get; set; }
        public uint PublicEventTeamId { get; set; }
        public uint LocalizedTextId { get; set; }
        public uint LocalizedTextIdOtherTeam { get; set; }
        public uint LocalizedTextIdShort { get; set; }
        public uint LocalizedTextIdOtherTeamShort { get; set; }
        public uint PublicEventObjectiveTypeEnum { get; set; }
        public uint Count { get; set; }
        public uint ObjectId { get; set; }
        public uint FailureTimeMs { get; set; }
        public uint TargetGroupIdRewardPane { get; set; }
        public uint PublicEventObjectiveCategoryEnum { get; set; }
        public uint LiveEventIdCounter { get; set; }
        public uint PublicEventObjectiveIdParent { get; set; }
        public uint QuestDirectionId { get; set; }
        public uint MedalPointValue { get; set; }
        public uint LocalizedTextIdParticipantAdd { get; set; }
        public uint LocalizedTextIdStart { get; set; }
        public uint DisplayOrder { get; set; }
    }
}
