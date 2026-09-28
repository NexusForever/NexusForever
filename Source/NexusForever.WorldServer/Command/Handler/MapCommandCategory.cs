using Microsoft.Extensions.DependencyInjection;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Abstract.Map.Lock;
using NexusForever.Game.Map;
using NexusForever.Game.Static.Map;
using NexusForever.Game.Static.RBAC;
using NexusForever.Shared;
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
            if (player.Map is not IMapInstance instance)
            {
                context.SendError("Current map is not an instance!");
                return;
            }

            instance.Unload();
        }

        // for retesting content that runs once per instance, such as an intro or a scripted event
        [Command(Permission.MapUnload, "Move into a fresh instance of the current world (drops your solo lock for it).", "fresh")]
        public void HandleMapFresh(ICommandContext context)
        {
            IPlayer player = context.GetTargetOrInvoker<IPlayer>();
            if (player.Map is not IMapInstance)
            {
                context.SendError("Current map is not an instance!");
                return;
            }

            ushort worldId = (ushort)player.Map.Entry.Id;

            // TODO: replace with dependency injection once commands system is refactored
            var mapLockManager = LegacyServiceProvider.Provider.GetService<IMapLockManager>();
            mapLockManager.RemoveSoloLock(player.Identity, worldId);

            player.TeleportTo(worldId, player.Position.X, player.Position.Y, player.Position.Z);
            context.SendMessage($"Moving into a fresh instance of world {worldId}. The old instance is left empty.");
        }

        [Command(Permission.MapPlayerRemove, "Remove player from current map instance.", "remove")]
        public void HandleMapPlayerRemove(ICommandContext context,
            [Parameter("Removal reason.", converter: typeof(EnumParameterConverter<WorldRemovalReason>))]
            WorldRemovalReason removalReason)
        {
            IPlayer player = context.GetTargetOrInvoker<IPlayer>();
            if (player.Map is not IMapInstance instance)
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
