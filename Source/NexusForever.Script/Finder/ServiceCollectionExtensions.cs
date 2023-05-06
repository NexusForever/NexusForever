using Microsoft.Extensions.DependencyInjection;

namespace NexusForever.Script.Finder
{
    public static class ServiceCollectionExtensions
    {
        public static void AddScriptFinder(this IServiceCollection sc)
        {
            sc.AddTransient<IAssemblyFinder, AssemblyFinder>();
            sc.AddTransient<ISourceFinder, SourceFinder>();
        }
    }
}
