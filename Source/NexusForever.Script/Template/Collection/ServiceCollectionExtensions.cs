using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Abstract.Spell;

namespace NexusForever.Script.Template.Collection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddScriptTemplateCollection(this IServiceCollection sc)
        {
            sc.AddTransient<ICollectionFactory, CollectionFactory>();

            sc.AddTransient<IOwnedScriptCollection<IBaseMap>, OwnedScriptCollection<IBaseMap>>();
            sc.AddTransient<IOwnedScriptCollection<INonPlayerEntity>, OwnedScriptCollection<INonPlayerEntity>>();
            sc.AddTransient<IOwnedScriptCollection<ICreatureEntity>, OwnedScriptCollection<ICreatureEntity>>();
            sc.AddTransient<IOwnedScriptCollection<IPlayer>, OwnedScriptCollection<IPlayer>>();
            sc.AddTransient<IOwnedScriptCollection<IQuest>, OwnedScriptCollection<IQuest>>();
            sc.AddTransient<IOwnedScriptCollection<ISpell>, OwnedScriptCollection<ISpell>>();
        }
    }
}
