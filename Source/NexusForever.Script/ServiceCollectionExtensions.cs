using Microsoft.Extensions.DependencyInjection;
using NexusForever.Script.Compile;
using NexusForever.Script.Finder;
using NexusForever.Script.Loader;
using NexusForever.Script.Template;
using NexusForever.Script.Watcher;
using NexusForever.Shared;

namespace NexusForever.Script
{
    public static class ServiceCollectionExtensions
    {
        public static void AddScript(this IServiceCollection sc)
        {
            sc.AddSingleton<IScriptManager, ScriptManager>();

            sc.AddTransientFactory<IScriptAssemblyInfo, ScriptAssemblyInfo>();
            sc.AddTransientFactory<IScriptInfo, ScriptInfo>();
            sc.AddTransientFactory<IScriptInstanceInfo, ScriptInstanceInfo>();

            sc.AddScriptCompile();
            sc.AddScriptFinder();
            sc.AddScriptLoader();
            sc.AddScriptTemplate();
            sc.AddScriptWatcher();
        }
    }
}
