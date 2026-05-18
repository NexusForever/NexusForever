using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace NexusForever.Script.Template.Event
{
    public static class ServiceCollectionExtensions
    {
        public static void AddScriptTemplateEvent(this IServiceCollection sc)
        {
            sc.AddTransient<IScriptEventManager, ScriptEventManager>();
            sc.AddTransient<IScriptEventFactory, ScriptEventFactory>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsInterface || !type.IsAssignableTo(typeof(IScriptEvent)))
                    continue;

                Type interfaceType = type.GetInterface($"I{type.Name}");
                if (interfaceType == null)
                    continue;

                sc.AddTransient(interfaceType, type);
            }
        }
    }
}
