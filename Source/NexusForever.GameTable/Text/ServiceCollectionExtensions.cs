using Microsoft.Extensions.DependencyInjection;
using NexusForever.GameTable.Text.Filter;
using NexusForever.GameTable.Text.Search;
using NexusForever.Shared;

namespace NexusForever.GameTable.Text
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameTableText(this IServiceCollection sc)
        {
            sc.AddSingletonLegacy<ITextFilterManager, TextFilterManager>();
            sc.AddSingletonLegacy<ISearchManager, SearchManager>();
        }
    }
}
