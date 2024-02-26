namespace NexusForever.GameTable.Model
{
    public class ModelSequenceTransitionEntry
    {
        public uint Id { get; set; }
        public uint ModelSequenceIdFrom { get; set; }
        public uint ModelSequenceIdTo { get; set; }
        public uint ModelSequenceIdTransition { get; set; }
    }
}
