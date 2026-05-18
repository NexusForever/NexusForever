using Microsoft.Extensions.DependencyInjection;
using NexusForever.Script.Template.Collection;
using NexusForever.Script.Template.Event;
using NexusForever.Script.Template.Filter;

namespace NexusForever.Script.Template
{
    public static class ServiceCollectionExtensions
    {
        public static void AddScriptTemplate(this IServiceCollection sc)
        {
            sc.AddScriptTemplateCollection();
            sc.AddScriptTemplateEvent();
            sc.AddScriptTemplateFilter();
        }
    }
}
