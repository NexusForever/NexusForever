using System.Collections.Generic;
using System.Numerics;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Convert;
using NexusForever.WorldServer.Command.Static;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Movement.Generator;
using NexusForever.WorldServer.Game.Entity.Movement.Spline.Static;
using NexusForever.WorldServer.Game.RBAC.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Movement, "A collection of commands to control entity movement.", "movement", "move")]
    [CommandTarget(typeof(WorldEntity))]
    public class MovementCommandCategory : CommandCategory
    {
        [Command(Permission.MovementSpline, "A collection of commands to control entity spline movement.", "spline")]
        public class MovementSplineCategory : CommandCategory
        {
            private static readonly Dictionary<uint, List<Vector3>> entityNodes = new Dictionary<uint, List<Vector3>>();

            [Command(Permission.MovementSplineAdd, "A position to target entity spine nodes.", "add")]
            public void MovementSplineAddHandler(ICommandContext context)
            {
                var entity = context.GetTargetOrInvoker<WorldEntity>();
                if (!entityNodes.ContainsKey(entity.Guid))
                {
                    entityNodes.Add(entity.Guid, new List<Vector3>());
                    entityNodes[entity.Guid].Add(entity.Position);
                }

                Vector3 position = context.Invoker.Position;
                entityNodes[entity.Guid].Add(position);
                
                context.SendMessage($"Added spline node X:{position.X}, Y:{position.Y}, Z:{position.Z} to entity {entity.Guid}.");
            }

            [Command(Permission.MovementSplineClear, "Clear all positions from target entity spline nodes.", "clear")]
            public void MovementSplineClearHandler(ICommandContext context)
            {
                var entity = context.GetTargetOrInvoker<WorldEntity>();
                if (!entityNodes.TryGetValue(entity.Guid, out List<Vector3> nodes))
                    return;

                entityNodes.Remove(entity.Guid);
                context.SendMessage($"Cleared {nodes.Count} spline nodes for entity {entity.Guid}.");
            }

            [Command(Permission.MovementSplineLaunch, "Launch spline for target entity with previously defined nodes and optional mode and speed.", "launch")]
            public void MovementSplineLaunchHandler(ICommandContext context,
                [Parameter("Mode to launch the spline.", ParameterFlags.None, typeof(EnumParameterConverter<SplineMode>))]
                SplineMode? mode,
                [Parameter("Speed to launch the spline.")]
                float? speed)
            {
                mode  ??= SplineMode.OneShot;
                speed ??= 3f;

                var entity = context.GetTargetOrInvoker<WorldEntity>();
                if (!entityNodes.TryGetValue(entity.Guid, out List<Vector3> nodes))
                {
                    context.SendMessage("Selected target entity has no nodes!");
                    return;
                }

                entity.MovementManager.LaunchSpline(nodes, SplineType.Linear, mode.Value, speed.Value);
                entityNodes.Remove(entity.Guid);

                context.SendMessage($"Launching spline for entity {entity.Guid} with {nodes.Count} nodes.");
            }
        }

        [Command(Permission.MovementGenerator, "A collection of commands to control entity generator movement.", "generator")]
        public class MovementGeneratorCategory : CommandCategory
        {
            [Command(Permission.MovementGeneratorDirect, "Launch spline for target entity with nodes defined by the direct movement generator.", "direct")]
            public void MovementGeneratorDirectHandler(ICommandContext context)
            {
                var entity = context.GetTargetOrInvoker<WorldEntity>();
                var generator = new DirectMovementGenerator
                {
                    Begin = entity.Position,
                    Final = context.Invoker.Position,
                    Map   = entity.Map
                };

                entity.MovementManager.LaunchGenerator(generator, 3f);
            }

            [Command(Permission.MovementGeneratorRandom, "Launch spline for target entity with nodes defined by the random movement generator.", "random")]
            public void MovementGeneratorRandomHandler(ICommandContext context)
            {
                var entity = context.GetTargetOrInvoker<WorldEntity>();
                var generator = new RandomMovementGenerator
                {
                    Begin = entity.Position,
                    Leash = entity.LeashPosition,
                    Range = entity.LeashRange,
                    Map   = entity.Map
                };

                entity.MovementManager.LaunchGenerator(generator, 3f);
            }
        }
    }
}
