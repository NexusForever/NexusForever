using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Entity.Movement;
using NexusForever.Shared;

namespace NexusForever.Game.Entity
{
    public static class ServiceCollectionExtentions
    {
        public static void AddGameEntity(this IServiceCollection sc)
        {
            sc.AddGameEntityMovement();

            sc.AddSingletonLegacy<IBuybackManager, BuybackManager>();
            sc.AddSingletonLegacy<IEntityManager, EntityManager>();
            sc.AddSingletonLegacy<IPlayerManager, PlayerManager>();
        }
    }
}
