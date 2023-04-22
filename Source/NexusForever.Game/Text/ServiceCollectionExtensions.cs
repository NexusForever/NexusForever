using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Text.Filter;
using NexusForever.Game.Abstract.Text.Search;
using NexusForever.Game.Text.Filter;
using NexusForever.Game.Text.Search;
using NexusForever.Shared;

namespace NexusForever.Game.Text
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameText(this IServiceCollection sc)
        {
            sc.AddSingletonLegacy<ITextFilterManager, TextFilterManager>();
            sc.AddSingletonLegacy<ISearchManager, SearchManager>();
        }
    }
}
