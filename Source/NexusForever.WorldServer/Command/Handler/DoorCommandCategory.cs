using System.Collections.Generic;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Map.Search;
using NexusForever.Game.Static.RBAC;
using NexusForever.WorldServer.Command.Context;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Door, "A collection of commands to interact with door entities.", "door")]
    [CommandTarget(typeof(IPlayer))]
    public class DoorCommandCategory : CommandCategory
    {
        [Command(Permission.DoorOpen, "Open all doors within a specified range.", "open")]
        public void HandleDoorOpen(ICommandContext context,
            [Parameter("Distance to search for doors to open.")]
            float? searchRange)
        {
            searchRange ??= 10f;

            IPlayer player = context.GetTargetOrInvoker<IPlayer>();

            var check = new SearchCheckRange<IDoorEntity>();
            check.Initialise(player.Position, searchRange.Value);

            IEnumerable<IDoorEntity> doors = player.Map.Search(
                player.Position,
                searchRange.Value,
                check);

            foreach (IDoorEntity door in doors)
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

            IPlayer player = context.GetTargetOrInvoker<IPlayer>();

            var check = new SearchCheckRange<IDoorEntity>();
            check.Initialise(player.Position, searchRange.Value);

            IEnumerable<IDoorEntity> doors = player.Map.Search(
                player.Position,
                searchRange.Value,
                check);

            foreach (IDoorEntity door in doors)
            {
                context.SendMessage($"Trying to close door {door.Guid}");
                door.CloseDoor();
            }
        }
    }
}
