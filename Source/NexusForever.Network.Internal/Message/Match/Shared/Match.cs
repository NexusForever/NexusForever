namespace NexusForever.Network.Internal.Message.Match.Shared
{
    public class Match
    {
        public Guid Guid { get; set; }
        public uint GameMapEntryId { get; set; }
        public uint GameTypeEntryId { get; set; }
        public List<MatchTeam> Teams { get; set; }
    }
}
