namespace NexusForever.Game.Abstract
{
    public record Identity
    {
        public required ushort RealmId { get; init; }
        public required ulong Id { get; init; }
    }
}
