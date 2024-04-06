using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Entity.Movement;
using NexusForever.Game.Abstract.Entity.Movement.AntiTamper;
using NexusForever.Game.Abstract.Entity.Movement.Generator;
using NexusForever.Game.Entity.Movement.AntiTamper;
using NexusForever.Game.Entity.Movement.Command;
using NexusForever.Game.Entity.Movement.Generator;
using NexusForever.Game.Entity.Movement.Spline;

namespace NexusForever.Game.Entity.Movement
{
    public static class ServiceCollectionExtentions
    {
        public static void AddGameEntityMovement(this IServiceCollection sc)
        {
            sc.AddGameEntityMovementCommand();
            sc.AddGameEntityMovementSpline();

            sc.AddTransient<IClientMovementCommandValidator, ClientMovementCommandValidator>();

            sc.AddTransient<IDirectMovementGenerator, DirectMovementGenerator>();
            sc.AddTransient<IPathMovementGenerator, PathMovementGenerator>();
            sc.AddTransient<IRandomMovementGenerator, RandomMovementGenerator>();

            sc.AddTransient<IMovementManager, MovementManager>();
        }
    }
}
