namespace NexusForever.API.Model
{
    public record Identity
    {
        public required ulong Id { get; init; }
        public required ushort RealmId { get; init; }
    }
}
