namespace NexusForever.Network.Internal.Message.Shared
{
    public record Identity
    {
        public ulong Id { get; init; }
        public ushort RealmId { get; init; }
    }
}
