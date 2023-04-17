using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.RBAC;
using NexusForever.Shared;

namespace NexusForever.Game.RBAC
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGameRbac(this IServiceCollection sc)
        {
            sc.AddSingletonLegacy<IRBACManager, RBACManager>();
        }
    }
}
