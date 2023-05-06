using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace NexusForever.Script.Template.Filter.Dynamic
{
    public static class ServiceCollectionExtensions
    {
        public static void AddScriptTemplateFilterDynamic(this IServiceCollection sc)
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsInterface || !type.IsAssignableTo(typeof(IScriptFilterDynamic)))
                    continue;

                Type interfaceType = type.GetInterface($"I{type.Name}");
                if (interfaceType == null)
                    continue;

                sc.AddTransient(interfaceType, type);
            }
        }
    }
}
