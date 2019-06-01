using System.Collections.Generic;
using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Map;
using NLog;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Door")]
    public class DoorCommandHandler : CommandCategory
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public DoorCommandHandler()
            : base(true, "door")
        {
        }

        [SubCommandHandler("open", "[range] - Open all doors within a given range (Defaults to 10m).")]
        public Task DoorOpenSubCommand(CommandContext context, string command, string[] parameters)
        {
            uint searchRange = 10;
            if (parameters.Length > 0)
                searchRange = uint.Parse(parameters[0]);

            context.Session.Player.Map.Search(
                context.Session.Player.Position,
                searchRange,
                new SearchCheckRange(context.Session.Player.Position, searchRange, context.Session.Player),
                out List<GridEntity> intersectedEntities
            );

            foreach(var entity in intersectedEntities)
            {
                if (entity is Door door)
                {
                    context.SendMessageAsync($"Trying to open door {door.Guid}");
                    door.OpenDoor(context.Session.Player);
                }
            }

            return Task.CompletedTask;
        }

        [SubCommandHandler("close", "[range] - Close all doors within a given range (Defaults to 10m).")]
        public Task DoorCloseSubCommand(CommandContext context, string command, string[] parameters)
        {
            uint searchRange = 10;
            if (parameters.Length > 0)
                searchRange = uint.Parse(parameters[0]);

            context.Session.Player.Map.Search(
                context.Session.Player.Position,
                searchRange,
                new SearchCheckRange(context.Session.Player.Position, searchRange, context.Session.Player),
                out List<GridEntity> intersectedEntities
            );

            foreach (var entity in intersectedEntities)
            {
                if (entity is Door door)
                {
                    context.SendMessageAsync($"Trying to close door {door.Guid}");
                    door.CloseDoor(context.Session.Player);
                }
            }

            return Task.CompletedTask;
        }
    }
}
