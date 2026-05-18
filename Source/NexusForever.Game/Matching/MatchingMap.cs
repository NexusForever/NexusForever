using NexusForever.Game.Abstract.Matching;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Matching
{
    public class MatchingMap : IMatchingMap
    {
        public uint Id => GameMapEntry.Id;
        public MatchingGameMapEntry GameMapEntry { get; init; }
        public MatchingGameTypeEntry GameTypeEntry { get; init; }
    }
}
