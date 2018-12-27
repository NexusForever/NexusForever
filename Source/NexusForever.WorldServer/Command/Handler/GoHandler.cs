using System.Linq;
using System.Threading.Tasks;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.GameTable.Static;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game;

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

            WorldLocation2Entry zone = SearchManager.Search<WorldLocation2Entry>(zoneName, context.Language, 
                i => 
                GameTableManager.WorldZone.GetEntry(i.WorldZoneId)?.LocalizedTextIdName ?? 
                GameTableManager.World.GetEntry(i.WorldId)?.LocalizedTextIdName ?? 
                0).FirstOrDefault();
            if (zone == null)
                await context.SendErrorAsync($"Unknown zone: {zoneName}");
            else
                context.Session.Player.TeleportTo((ushort)zone.WorldId, zone.Position0, zone.Position1,
                    zone.Position2);
        }
    }
}
