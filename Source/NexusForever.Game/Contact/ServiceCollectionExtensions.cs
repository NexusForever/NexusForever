using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Contact;
using NexusForever.Shared;

namespace NexusForever.Game.Contact
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameContact(this IServiceCollection sc)
        {
            sc.AddSingletonLegacy<IGlobalContactManager, GlobalContactManager>();
        }
    }
}
