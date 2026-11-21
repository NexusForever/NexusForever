namespace NexusForever.Database.Query.Repository.Query
{
    public class Query
    {
        public uint MaxResults { get; set; }
        public ushort RealmId { get; set; }
        public List<QueryGroup> Groups { get; set; } = [];
    }
}
