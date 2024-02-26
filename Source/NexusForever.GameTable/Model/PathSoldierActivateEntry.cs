namespace NexusForever.GameTable.Model
{
    public class PathSoldierActivateEntry
    {
        public uint Id { get; set; }
        public uint Creature2Id { get; set; }
        public uint TargetGroupId { get; set; }
        public uint Count { get; set; }
        public uint SoldierActivateModeEnum { get; set; }
    }
}
