namespace NexusForever.GameTable.Model
{
    public class ItemRandomStatEntry
    {
        public uint Id { get; set; }
        public uint ItemRandomStatGroupId { get; set; }
        public float Weight { get; set; }
        public uint ItemStatTypeEnum { get; set; }
        public uint ItemStatData { get; set; }
    }
}
