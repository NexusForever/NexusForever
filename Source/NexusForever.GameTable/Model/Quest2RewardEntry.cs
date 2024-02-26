namespace NexusForever.GameTable.Model
{
    public class Quest2RewardEntry
    {
        public uint Id { get; set; }
        public uint Quest2Id { get; set; }
        public uint Quest2RewardTypeId { get; set; }
        public uint ObjectId { get; set; }
        public uint ObjectAmount { get; set; }
        public uint Flags { get; set; }
    }
}
