using NexusForever.Game.Static.Matching;

namespace NexusForever.Game.Abstract.Matching.Match
{
    public interface IPvpMatch : IMatch
    {
        /// <summary>
        /// Set the state of the match to the supplied <see cref="PvpGameState"/>.
        /// </summary>
        void SetState(PvpGameState state);

        /// <summary>
        /// Update deathmatch pool for the team the character is on.
        /// </summary>
        void UpdatePool(ulong characterId);

        /// <summary>
        /// Finish the match with the supplied <see cref="MatchWinner"/> and <see cref="MatchEndReason"/>
        /// </summary>
        void MatchFinish(MatchWinner matchWinner, MatchEndReason matchEndReason);
    }
}
