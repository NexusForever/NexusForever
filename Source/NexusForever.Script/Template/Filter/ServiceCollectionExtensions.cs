using Microsoft.Extensions.DependencyInjection;
using NexusForever.Script.Template.Filter.Dynamic;

namespace NexusForever.Script.Template.Filter
{
    public static class ServiceCollectionExtensions
    {
        public static void AddScriptTemplateFilter(this IServiceCollection sc)
        {
            sc.AddTransient<IScriptFilterParameters, ScriptFilterParameters>();

            sc.AddScriptTemplateFilterDynamic();
        }
    }
}
