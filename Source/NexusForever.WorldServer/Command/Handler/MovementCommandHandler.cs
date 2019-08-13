using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Movement.Generator;
using NexusForever.WorldServer.Game.Entity.Movement.Spline.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Movement")]
    public class MovementCommandHandler : CommandCategory
    {
        private static readonly Dictionary<uint, List<Vector3>> entityNodes = new Dictionary<uint, List<Vector3>>();

        public MovementCommandHandler()
            : base(true, "movement", "move")
        {
        }

        [SubCommandHandler("direct", "")]
        public async Task DebugDirectGenerator(CommandContext context, string command, string[] parameters)
        {
            WorldEntity entity = context.Session.Player.GetVisible<WorldEntity>(context.Session.Player.TargetGuid);
            if (entity == null)
            {
                await context.SendMessageAsync("Select a valid target entity!");
                return;
            }

            var generator = new DirectMovementGenerator
            {
                Begin = entity.Position,
                Final = context.Session.Player.Position,
                Map   = entity.Map
            };

            entity.MovementManager.LaunchGenerator(generator, 3f);
        }

        [SubCommandHandler("random", "")]
        public async Task DebugRandomGenerator(CommandContext context, string command, string[] parameters)
        {
            WorldEntity entity = context.Session.Player.GetVisible<WorldEntity>(context.Session.Player.TargetGuid);
            if (entity == null)
            {
                await context.SendMessageAsync("Select a valid target entity!");
                return;
            }

            var generator = new RandomMovementGenerator
            {
                Begin = entity.Position,
                Leash = entity.LeashPosition,
                Range = entity.LeashRange,
                Map   = entity.Map
            };

            entity.MovementManager.LaunchGenerator(generator, 3f);
        }

        [SubCommandHandler("splineadd", "")]
        public async Task DebugSplineAddCommandHandler(CommandContext context, string command, string[] parameters)
        {
            WorldEntity entity = context.Session.Player.GetVisible<WorldEntity>(context.Session.Player.TargetGuid);
            if (entity == null)
            {
                await context.SendMessageAsync("Select a valid target entity!");
                return;
            }

            if (!entityNodes.ContainsKey(entity.Guid))
            {
                entityNodes.Add(entity.Guid, new List<Vector3>());
                entityNodes[entity.Guid].Add(entity.Position);
            }

            Vector3 position = context.Session.Player.Position;
            entityNodes[entity.Guid].Add(position);

            await context.SendMessageAsync($"Added spline node X:{position.X}, Y:{position.Y}, Z:{position.Z} to entity {entity.Guid}.");
        }

        [SubCommandHandler("splineclear", "")]
        public async Task DebugSplineClearCommandHandler(CommandContext context, string command, string[] parameters)
        {
            WorldEntity entity = context.Session.Player.GetVisible<WorldEntity>(context.Session.Player.TargetGuid);
            if (entity == null)
            {
                await context.SendMessageAsync("Select a valid target entity!");
                return;
            }

            if (!entityNodes.TryGetValue(entity.Guid, out List<Vector3> nodes))
                return;

            entityNodes.Remove(entity.Guid);
            await context.SendMessageAsync($"Cleared {nodes.Count} spline nodes for entity {entity.Guid}.");
        }

        [SubCommandHandler("splinelaunch", "")]
        public async Task DebugSplineLaunchCommandHandler(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length != 0 && parameters.Length != 2)
            {
                await SendHelpAsync(context);
                return;
            }

            WorldEntity entity = context.Session.Player.GetVisible<WorldEntity>(context.Session.Player.TargetGuid);
            if (entity == null)
            {
                await context.SendMessageAsync("Select a valid target entity!");
                return;
            }

            if (!entityNodes.TryGetValue(entity.Guid, out List<Vector3> nodes))
            {
                await context.SendMessageAsync("Selected target entity has no nodes!");
                return;
            }

            SplineMode mode = parameters.Length == 0 ? SplineMode.OneShot : (SplineMode)byte.Parse(parameters[0]);
            float speed = parameters.Length == 0 ? 3f : float.Parse(parameters[1]);

            entity.MovementManager.LaunchSpline(nodes, SplineType.Linear, mode, speed);
            entityNodes.Remove(entity.Guid);

            await context.SendMessageAsync($"Launching spline for entity {entity.Guid} with {nodes.Count} nodes.");
        }
    }
}
