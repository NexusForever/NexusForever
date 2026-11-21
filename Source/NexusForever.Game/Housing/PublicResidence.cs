using NexusForever.Game.Abstract.Housing;
using NexusForever.Game.Abstract;

namespace NexusForever.Game.Housing
{
    public class PublicResidence : IPublicResidence
    {
        public Identity Identity { get; init; }
        public string Owner { get; init; }
        public string Name { get; init; }
    }
}
