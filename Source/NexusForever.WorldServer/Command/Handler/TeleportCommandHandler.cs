using System.Numerics;
using System.Threading.Tasks;
using NexusForever.Shared;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Teleport")]
    public class TeleportCommandHandler : CommandCategory
    {
        public TeleportCommandHandler()
            : base(true, "teleport", "port")
        {
        }

        [SubCommandHandler("coordinates", "x, y, z, [worldId] - Teleport to the specified coordinates optionally specifying the world.")]
        public async Task TeleportCoordinatesSubCommandHandler(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length < 3 || parameters.Length > 4
                || !float.TryParse(parameters[0], out float x)
                || !float.TryParse(parameters[1], out float y)
                || !float.TryParse(parameters[2], out float z))
            {
                await SendHelpAsync(context);
                return;
            }

            if (parameters.Length == 4)
            {
                // optional world parameter is supplied, make sure it is valid too
                if (!ushort.TryParse(parameters[3], out ushort worldId))
                {
                    await SendHelpAsync(context);
                    return;
                }

                context.Session.Player.TeleportTo(worldId, x, y, z);
            }
            else
                context.Session.Player.TeleportTo((ushort)context.Session.Player.Map.Entry.Id, x, y, z);
        }

        [SubCommandHandler("location", "worldLocation2Id - Teleport to the specified world location.")]
        public async Task TeleportLocationSubCommandHandler(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length != 1 || !uint.TryParse(parameters[0], out uint worldLocation2Id))
            {
                await SendHelpAsync(context);
                return;
            }

            WorldLocation2Entry entry = GameTableManager.Instance.WorldLocation2.GetEntry(worldLocation2Id);
            if (entry == null)
            {
                await context.SendMessageAsync($"WorldLocation2 entry not found: {worldLocation2Id}");
                return;
            }

            var rotation = new Quaternion(entry.Facing0, entry.Facing1, entry.Facing2, entry.Facing3);
            context.Session.Player.Rotation = rotation.ToEulerDegrees();
            context.Session.Player.TeleportTo((ushort)entry.WorldId, entry.Position0, entry.Position1, entry.Position2);
        }
    }
}
