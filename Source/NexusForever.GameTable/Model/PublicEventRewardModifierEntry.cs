namespace NexusForever.GameTable.Model
{
    public class PublicEventRewardModifierEntry
    {
        public uint Id { get; set; }
        public uint PublicEventId { get; set; }
        public uint RewardPropertyId { get; set; }
        public uint Data { get; set; }
        public float Offset { get; set; }
    }
}
