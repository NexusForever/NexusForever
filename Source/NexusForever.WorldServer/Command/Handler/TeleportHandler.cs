using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Network;

namespace NexusForever.WorldServer.Command.Handler
{

    public class GoHandler : NamedCommand
    {

        public GoHandler(ILogger<GoHandler> logger)
            : base(new[] { "go" }, true, logger)
        {
        }

        protected override async Task HandleCommandAsync(CommandContext context, string command, string[] parameters)
        {
            string zoneName = string.Join(" ", parameters);
            WorldLocation2Entry zone = GameTableManager.LookupZonesByName(zoneName).FirstOrDefault();
            if (zone == null)
            {
                await context.SendErrorAsync(Logger, $"Unknown zone: {zoneName}");
            }
            else
            {
                context.Session.Player.TeleportTo((ushort)zone.WorldId, zone.Position0, zone.Position1, zone.Position2);
            }
        }
    }

    [Name("Teleport")]
    public class TeleportHandler : NamedCommand
    {
        public TeleportHandler(ILogger<TeleportHandler> logger)
            : base(new[] {"teleport", "port"}, true, logger)
        {
        }

        protected override Task HandleCommandAsync(CommandContext context, string command, string[] parameters)
        {
            WorldSession session = context.Session;
            if (parameters.Length == 4)
                session.Player.TeleportTo(ushort.Parse(parameters[0]), float.Parse(parameters[1]),
                    float.Parse(parameters[2]), float.Parse(parameters[3]));
            else if (parameters.Length == 3)
                session.Player.TeleportTo((ushort) session.Player.Map.Entry.Id, float.Parse(parameters[0]),
                    float.Parse(parameters[1]), float.Parse(parameters[2]));

            return Task.CompletedTask;
        }
    }
}
