using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Entity;
using NexusForever.Game.Map.Instance;
using NexusForever.Game.Static.Map;
using NexusForever.Game.Static.RBAC;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Convert;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Map, "A collection of commands to manage maps.", "map")]
    [CommandTarget(typeof(IPlayer))]
    public class MapCommandCategory : CommandCategory
    {
        [Command(Permission.MapUnload, "Unload current map instance.", "unload")]
        public void HandleMapUnload(ICommandContext context)
        {
            IPlayer player = context.GetTargetOrInvoker<IPlayer>();
            if (player.Map is not MapInstance instance)
            {
                context.SendError("Current map is not an instance!");
                return;
            }

            instance.Unload();
        }

        [Command(Permission.MapPlayerRemove, "Remove player from current map instance.", "remove")]
        public void HandleMapPlayerRemove(ICommandContext context,
            [Parameter("Removal reason.", converter: typeof(EnumParameterConverter<WorldRemovalReason>))]
            WorldRemovalReason removalReason)
        {
            IPlayer player = context.GetTargetOrInvoker<IPlayer>();
            if (player.Map is not MapInstance instance)
            {
                context.SendError("Current map is not an instance!");
                return;
            }

            instance.EnqueuePendingRemoval(player, removalReason);
        }

        [Command(Permission.MapPlayerRemoveCancel, "Cancel removal of player from current map instance.", "cancel")]
        public void HandleMapPlayerRemoveCancel(ICommandContext context)
        {
            IPlayer player = context.GetTargetOrInvoker<IPlayer>();
            if (player.Map is not IMapInstance instance)
            {
                context.SendError("Current map is not an instance!");
                return;
            }

            instance.CancelPendingRemoval(player);
        }
    }
}
