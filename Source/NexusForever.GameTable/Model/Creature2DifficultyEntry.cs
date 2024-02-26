namespace NexusForever.GameTable.Model
{
    public class Creature2DifficultyEntry
    {
        public uint Id { get; set; }
        public uint LocalizedTextIdTitle { get; set; }
        public float PathScientistScanMultiplier { get; set; }
        public float ClusterContributionValueDifficulty { get; set; }
        public uint RankValue { get; set; }
        public uint GroupValue { get; set; }
        [GameTableFieldArray(200)]
        public float[] UnitPropertyMultiplier { get; set; }
    }
}
