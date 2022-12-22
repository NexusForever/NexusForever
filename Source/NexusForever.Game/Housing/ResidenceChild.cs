namespace NexusForever.Game.Housing
{
    public class ResidenceChild
    {
        public Residence Residence { get; init; }
        public bool IsTemporary { get; set; }
        public DateTime? RemovalTime { get; set; }
    }
}
