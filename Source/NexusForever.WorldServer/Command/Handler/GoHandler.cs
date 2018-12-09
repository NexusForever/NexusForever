using System.Linq;
using System.Threading.Tasks;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Command.Contexts;

namespace NexusForever.WorldServer.Command.Handler
{
    public class GoHandler : NamedCommand
    {
        public GoHandler()
            : base(true, "go")
        {
        }

        protected override async Task HandleCommandAsync(CommandContext context, string command, string[] parameters)
        {
            string zoneName = string.Join(" ", parameters);
            WorldLocation2Entry zone = GameTableManager.LookupZonesByName(zoneName).FirstOrDefault();
            if (zone == null)
                await context.SendErrorAsync($"Unknown zone: {zoneName}");
            else
            {
                await context.SendMessageAsync(
                    $"Teleporting to {zone.WorldId} {zone.Position0} {zone.Position1} {zone.Position2}");
                context.Session.Player.TeleportTo((ushort) zone.WorldId, zone.Position0, zone.Position1,
                    zone.Position2);
        }
    }
}
