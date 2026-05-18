using Microsoft.Extensions.DependencyInjection;

namespace NexusForever.Script.Template.Collection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddScriptTemplateCollection(this IServiceCollection sc)
        {
            sc.AddTransient<ICollectionFactory, CollectionFactory>();

            sc.AddTransient(typeof(IOwnedScriptCollection<>), typeof(OwnedScriptCollection<>));
        }
    }
}
