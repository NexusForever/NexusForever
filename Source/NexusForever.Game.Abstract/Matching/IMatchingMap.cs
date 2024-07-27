using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Matching
{
    public interface IMatchingMap
    {
        uint Id { get; }
        MatchingGameMapEntry GameMapEntry { get; init; }
        MatchingGameTypeEntry GameTypeEntry { get; init; }
    }
}