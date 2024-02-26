namespace NexusForever.GameTable.Model
{
    public class EmoteSequenceTransitionEntry
    {
        public uint Id { get; set; }
        public uint EmotesIdTo { get; set; }
        public uint StandStateFrom { get; set; }
        public uint EmotesIdFrom { get; set; }
        public uint ModelSequenceId { get; set; }
    }
}
