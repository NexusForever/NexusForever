using System.Collections.Generic;
using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Map.Search;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Door")]
    public class DoorCommandHandler : CommandCategory
    {
        public DoorCommandHandler()
            : base(true, "door")
        {
        }

        [SubCommandHandler("open", "[range] - Open all doors within a given range (Defaults to 10m).")]
        public Task DoorOpenSubCommand(CommandContext context, string command, string[] parameters)
        {
            float searchRange = 10f;
            if (parameters.Length > 0)
                searchRange = float.Parse(parameters[0]);

            context.Session.Player.Map.Search(
                context.Session.Player.Position,
                searchRange,
                new SearchCheckRangeDoorOnly(context.Session.Player.Position, searchRange, context.Session.Player),
                out List<GridEntity> intersectedEntities
            );

            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach (Door door in intersectedEntities)
            {
                context.SendMessageAsync($"Trying to open door {door.Guid}");
                door.OpenDoor();
            }

            return Task.CompletedTask;
        }

        [SubCommandHandler("close", "[range] - Close all doors within a given range (Defaults to 10m).")]
        public Task DoorCloseSubCommand(CommandContext context, string command, string[] parameters)
        {
            float searchRange = 10f;
            if (parameters.Length > 0)
                searchRange = float.Parse(parameters[0]);

            context.Session.Player.Map.Search(
                context.Session.Player.Position,
                searchRange,
                new SearchCheckRangeDoorOnly(context.Session.Player.Position, searchRange, context.Session.Player),
                out List<GridEntity> intersectedEntities
            );

            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach (Door door in intersectedEntities)
            {
                context.SendMessageAsync($"Trying to close door {door.Guid}");
                door.CloseDoor();
            }

            return Task.CompletedTask;
        }
    }
}
