using NexusForever.Game.Abstract.Housing;

namespace NexusForever.Game.Housing
{
    public class PublicCommunity : IPublicCommunity
    {
        public ulong NeighbourhoodId { get; init; }
        public string Owner { get; init; }
        public string Name { get; init; }
    }
}
