namespace NexusForever.Server.GroupServer
{
    public record Identity
    {
        public required ulong Id { get; init; }
        public required ushort RealmId { get; init; }
    }
}
