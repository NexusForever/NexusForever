using NexusForever.Game.Abstract.Matching.Match;
using NexusForever.Game.Static.Matching;

namespace NexusForever.Script.Template
{
    public interface IContentPvpMapScript : IContentMapScript
    {
        /// <summary>
        /// Invoked when the <see cref="IPvpMatch"/> for the map changes <see cref="PvpGameState"/>.
        /// </summary>
        void OnPvpMatchState(PvpGameState state)
        {
        }

        /// <summary>
        /// Invoked when the <see cref="IPvpMatch"/> for the map finishes.
        /// </summary>
        void OnPvpMatchFinish(MatchWinner matchWinner, MatchEndReason matchEndReason)
        {
        }
    }
}
