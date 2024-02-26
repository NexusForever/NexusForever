namespace NexusForever.GameTable.Model
{
    public class ChallengeEntry
    {
        public uint Id { get; set; }
        public uint ChallengeTypeEnum { get; set; }
        public uint Target { get; set; }
        public uint ChallengeFlags { get; set; }
        public uint WorldZoneIdRestriction { get; set; }
        public uint TriggerVolume2IdRestriction { get; set; }
        public uint WorldZoneId { get; set; }
        public uint WorldLocation2IdIndicator { get; set; }
        public uint WorldLocation2IdStartLocation { get; set; }
        public uint CompletionCount { get; set; }
        public uint ChallengeTierId00 { get; set; }
        public uint ChallengeTierId01 { get; set; }
        public uint ChallengeTierId02 { get; set; }
        public uint LocalizedTextIdName { get; set; }
        public uint LocalizedTextIdProgress { get; set; }
        public uint LocalizedTextIdAreaRestriction { get; set; }
        public uint LocalizedTextIdLocation { get; set; }
        public uint VirtualItemIdDisplay { get; set; }
        public uint TargetGroupIdRewardPane { get; set; }
        public uint QuestDirectionIdActive { get; set; }
        public uint QuestDirectionIdInactive { get; set; }
        public uint RewardTrackId { get; set; }
    }
}
