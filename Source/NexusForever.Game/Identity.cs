using NexusForever.Game.Abstract;

namespace NexusForever.Game
{
    public record Identity : IIdentity
    {
        public ushort RealmId { get; set; }
        public ulong Id { get; set; }
    }
}
