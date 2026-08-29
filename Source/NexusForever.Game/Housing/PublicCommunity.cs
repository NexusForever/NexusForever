using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Housing;

namespace NexusForever.Game.Housing
{
    public class PublicCommunity : IPublicCommunity
    {
        public Identity GuildIdentity { get; init; }
        public string Owner { get; init; }
        public string Name { get; init; }
    }
}
