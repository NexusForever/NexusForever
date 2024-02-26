namespace NexusForever.GameTable.Model
{
    public class PathSettlerImprovementGroupEntry
    {
        public uint Id { get; set; }
        public uint PathSettlerHubId { get; set; }
        public uint PathSettlerImprovementGroupFlags { get; set; }
        public uint Creature2IdDepot { get; set; }
        public uint LocalizedTextIdName { get; set; }
        public uint SettlerAvenueTypeEnum { get; set; }
        public uint ContributionValue { get; set; }
        public uint PerTierBonusContributionValue { get; set; }
        public uint DurationPerBundleMs { get; set; }
        public uint MaxBundleCount { get; set; }
        public uint PathSettlerImprovementGroupIdOutpostRequired { get; set; }
        public uint PathSettlerImprovementIdTier00 { get; set; }
        public uint PathSettlerImprovementIdTier01 { get; set; }
        public uint PathSettlerImprovementIdTier02 { get; set; }
        public uint PathSettlerImprovementIdTier03 { get; set; }
        public uint WorldLocation2IdDisplayPoint { get; set; }
    }
}
