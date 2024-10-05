using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Map.Search;

namespace NexusForever.Game.Map.Search
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameMapSearch(this IServiceCollection sc)
        {
            sc.AddTransient<ISearchCheckFactory, SearchCheckFactory>();
            sc.AddTransient(typeof(ISearchCheckRange<>), typeof(SearchCheckRange<>));
        }
    }
}
