using System.Collections.Generic;
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
            : base(false, "go")
        {
        }

        private IEnumerable<uint> GetTextIds(WorldLocation2Entry entry)
        {
            WorldZoneEntry worldZone = GameTableManager.Instance.WorldZone.GetEntry(entry.WorldZoneId);
            if (worldZone != null && worldZone.LocalizedTextIdName != 0)
                yield return worldZone.LocalizedTextIdName;
            WorldEntry world = GameTableManager.Instance.World.GetEntry(entry.WorldId);
            if (world != null && world.LocalizedTextIdName != 0)
                yield return world.LocalizedTextIdName;
        }
        protected override async Task HandleCommandAsync(CommandContext context, string command, string[] parameters)
        {
            string zoneName = string.Join(" ", parameters);

            WorldLocation2Entry zone = SearchManager.Instance.Search<WorldLocation2Entry>(zoneName, context.Language, GetTextIds).FirstOrDefault();

            if (zone == null)
                await context.SendErrorAsync($"Unknown zone: {zoneName}");
            else
            {
                context.Session?.Player.TeleportTo((ushort)zone.WorldId, zone.Position0, zone.Position1, zone.Position2);
                await context.SendMessageAsync($"{zoneName}: {zone.WorldId} {zone.Position0} {zone.Position1} {zone.Position2}");
            }
        }
    }
}
