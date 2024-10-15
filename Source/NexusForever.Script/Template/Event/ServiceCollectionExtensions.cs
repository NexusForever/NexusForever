using Microsoft.Extensions.DependencyInjection;

namespace NexusForever.Script.Template.Event
{
    public static class ServiceCollectionExtensions
    {
        public static void AddScriptTemplateEvent(this IServiceCollection sc)
        {
            sc.AddTransient<IScriptEventManager, ScriptEventManager>();
            sc.AddTransient<IScriptEventFactory, ScriptEventFactory>();

            sc.AddTransient<IEntitySayEvent, EntitySayEvent>();
            sc.AddTransient<IEntityRandomMovementEvent, EntityRandomMovementEvent>();
            sc.AddTransient(typeof(IEntitySummonEvent<>), typeof(EntitySummonEvent<>));
        }
    }
}
