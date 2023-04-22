using NexusForever.Game.Abstract.Housing;

namespace NexusForever.Game.Housing
{
    public class PublicResidence : IPublicResidence
    {
        public ulong ResidenceId { get; init; }
        public string Owner { get; init; }
        public string Name { get; init; }
    }
}
