using System.Collections.Generic;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Map.Search;
using NexusForever.WorldServer.Game.RBAC.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Door, "A collection of commands to interact with door entities.", "door")]
    [CommandTarget(typeof(Player))]
    public class DoorCommandCategory : CommandCategory
    {
        [Command(Permission.DoorOpen, "Open all doors within a specified range.", "open")]
        public void HandleDoorOpen(ICommandContext context,
            [Parameter("Distance to search for doors to open.")]
            float? searchRange)
        {
            searchRange ??= 10f;

            Player player = context.GetTargetOrInvoker<Player>();
            player.Map.Search(
                player.Position,
                searchRange.Value,
                new SearchCheckRangeDoorOnly(player.Position, searchRange.Value, player),
                out List<GridEntity> intersectedEntities
            );

            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach (Door door in intersectedEntities)
            {
                context.SendMessage($"Trying to open door {door.Guid}");
                door.OpenDoor();
            }
        }

        [Command(Permission.DoorClose, "Close all doors within a specified range.", "close")]
        public void HandleDoorClose(ICommandContext context,
            [Parameter("Distance to search for doors to close.")]
            float? searchRange)
        {
            searchRange ??= 10f;

            Player player = context.GetTargetOrInvoker<Player>();
            player.Map.Search(
                player.Position,
                searchRange.Value,
                new SearchCheckRangeDoorOnly(player.Position, searchRange.Value, player),
                out List<GridEntity> intersectedEntities
            );

            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach (Door door in intersectedEntities)
            {
                context.SendMessage($"Trying to close door {door.Guid}");
                door.CloseDoor();
            }
        }
    }
}
