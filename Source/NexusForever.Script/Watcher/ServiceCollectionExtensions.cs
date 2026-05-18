using Microsoft.Extensions.DependencyInjection;

namespace NexusForever.Script.Watcher
{
    public static class ServiceCollectionExtensions
    {
        public static void AddScriptWatcher(this IServiceCollection sc)
        {
            sc.AddTransient<IAssemblyWatcher, AssemblyWatcher>();
            sc.AddTransient<ISourceWatcher, SourceWatcher>();
        }
    }
}
