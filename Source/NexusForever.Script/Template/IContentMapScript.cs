using NexusForever.Game.Abstract.Matching.Match;

namespace NexusForever.Script.Template
{
    public interface IContentMapScript : IInstancedMapScript
    {
        /// <summary>
        /// Invoked when the <see cref="IMatch"/> for the map finishes.
        /// </summary>
        /// <remarks>
        /// This is invoked for all match types, for PvP matches this is invoked in addition to <see cref="IContentPvpMapScript.OnPvpMatchFinish(Game.Static.Matching.MatchWinner, Game.Static.Matching.MatchEndReason)"/>.
        /// </remarks>
        void OnMatchFinish()
        {
        }
    }
}
