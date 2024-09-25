using NexusForever.Game.Abstract.Matching.Match;

namespace NexusForever.Game.Abstract.Map.Instance
{
    public interface IContentMapInstance : IMapInstance
    {
        /// <summary>
        /// Active <see cref="IMatch"/> for map.
        /// </summary>
        IMatch Match { get; }

        /// <summary>
        /// Add <see cref="IMatch"/> for map.
        /// </summary>
        void SetMatch(IMatch match);

        /// <summary>
        /// Remove <see cref="IMatch"/> for map.
        /// </summary>
        void RemoveMatch();

        /// <summary>
        /// Invoked when the <see cref="IMatch"/> for the map finishes.
        /// </summary>
        void OnMatchFinish();
    }
}
