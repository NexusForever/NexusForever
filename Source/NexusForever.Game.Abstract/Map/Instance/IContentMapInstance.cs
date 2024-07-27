using NexusForever.Game.Abstract.Matching.Match;

namespace NexusForever.Game.Abstract.Map.Instance
{
    public interface IContentMapInstance : IMapInstance
    {
        /// <summary>
        /// Initialise <see cref="IContentMapInstance"/> with supplied <see cref="IMatch"/>.
        /// </summary>
        void Initialise(IMatch match);
    }
}
