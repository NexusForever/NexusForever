namespace NexusForever.GameTable.Model
{
    public class PathSoldierEventEntry
    {
        public uint Id { get; set; }
        public uint PathSoldierEventType { get; set; }
        public uint MaxTimeBetweenWaves { get; set; }
        public uint MaxEventTime { get; set; }
        public uint TowerDefenseAllowance { get; set; }
        public uint TowerDefenseBuildTimeMS { get; set; }
        public uint InitialSpawnTime { get; set; }
    }
}
