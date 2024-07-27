using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Abstract.Matching.Match;

namespace NexusForever.Game.Map.Instance
{
    public class ContentMapInstance : MapInstance, IContentMapInstance
    {
        private IMatch match;

        #region Dependency Injection

        private readonly IMatchManager matchManager;

        public ContentMapInstance(
            IMatchManager matchManager)
        {
            this.matchManager = matchManager;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="IContentMapInstance"/> with supplied <see cref="IMatch"/>.
        /// </summary>
        public void Initialise(IMatch match)
        {
            if (this.match != null)
                throw new InvalidOperationException();

            this.match = match;
        }

        protected override IMapPosition GetPlayerReturnLocation(IPlayer player)
        {
            if (match != null)
            {
                IMapPosition mapPosition = match.GetReturnPosition(player);
                if (mapPosition != null)
                    return mapPosition;
            }

            // TODO: fallback to default return location
            // maybe recall position?

            return null;
        }

        protected override void AddEntity(IGridEntity entity, Vector3 vector)
        {
            base.AddEntity(entity, vector);

            if (entity is IPlayer player)
                match?.MatchEnter(player);
        }

        protected override void RemoveEntity(IGridEntity entity)
        {
            if (entity is IPlayer player)
                match?.MatchExit(player);

            base.RemoveEntity(entity);
        }
    }
}
