using Microsoft.Extensions.DependencyInjection;

namespace NexusForever.Script.Template.Collection
{
    public class CollectionFactory : ICollectionFactory
    {
        private readonly IServiceProvider serviceProvider;

        public CollectionFactory(
            IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IScriptCollection CreateCollection()
        {
            return serviceProvider.GetRequiredService<IScriptCollection>();
        }

        public IOwnedScriptCollection<T> CreateOwnedCollection<T>()
        {
            return serviceProvider.GetRequiredService<IOwnedScriptCollection<T>>();
        }
    }
}
