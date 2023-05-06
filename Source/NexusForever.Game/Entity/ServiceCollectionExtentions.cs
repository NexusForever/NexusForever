using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Entity.Movement.Generator;
using NexusForever.Game.Entity.Movement;
using NexusForever.Game.Entity.Movement.Generator;
using NexusForever.Shared;

namespace NexusForever.Game.Entity
{
    public static class ServiceCollectionExtentions
    {
        public static void AddGameEntity(this IServiceCollection sc)
        {
            sc.AddTransient<IDirectMovementGenerator, DirectMovementGenerator>();
            sc.AddTransient<IRandomMovementGenerator, RandomMovementGenerator>();
            sc.AddSingletonLegacy<IGlobalMovementManager, GlobalMovementManager>();

            sc.AddSingletonLegacy<IBuybackManager, BuybackManager>();
            sc.AddSingletonLegacy<IEntityManager, EntityManager>();
            sc.AddSingletonLegacy<IPlayerManager, PlayerManager>();
        }
    }
}
