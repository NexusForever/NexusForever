namespace NexusForever.Shared.GameTable.Model
{
    public class Creature2DifficultyEntry
    {
        public uint Id;
        public uint LocalizedTextIdTitle;
        public float PathScientistScanMultiplier;
        public float ClusterContributionValueDifficulty;
        public uint RankValue;
        public uint GroupValue;
        [GameTableFieldArray(200)]
        public float[] UnitPropertyMultiplier;
    }
}
