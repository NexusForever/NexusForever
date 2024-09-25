using NexusForever.Game.Static.Matching;

namespace NexusForever.Game.Abstract.Map.Instance
{
    public interface IContentPvpMapInstance : IContentMapInstance
    {
        /// <summary>
        /// Invoked when the <see cref="IPvpMatch"/> for the map changes <see cref="PvpGameState"/>.
        /// </summary>
        void OnPvpMatchState(PvpGameState state);

        /// <summary>
        /// Invoked when the <see cref="IPvpMatch"/> for the map finishes.
        /// </summary>
        void OnPvpMatchFinish(MatchWinner matchWinner, MatchEndReason matchEndReason);
    }
}
