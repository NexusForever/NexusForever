using Microsoft.Extensions.DependencyInjection;
using NexusForever.Shared;

namespace NexusForever.Script.Loader
{
    public static class ServiceCollectionExtensions
    {
        public static void AddScriptLoader(this IServiceCollection sc)
        {
            sc.AddTransientFactory<IAssemblyLoader, AssemblyLoader>();
            sc.AddTransientFactory<ISourceLoader, SourceLoader>();
        }
    }
}
