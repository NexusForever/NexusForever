using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map.Search;

namespace NexusForever.Game.Map.Search
{
    public class SearchCheckFactory : ISearchCheckFactory
    {
        #region Dependency Injection

        private readonly IServiceProvider serviceProvider;

        public SearchCheckFactory(
            IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        #endregion

        public TCheck CreateCheck<TEntity, TCheck>()
            where TEntity : IGridEntity
            where TCheck : ISearchCheck<TEntity>
        {
            return serviceProvider.GetService<TCheck>();
        }
    }
}
