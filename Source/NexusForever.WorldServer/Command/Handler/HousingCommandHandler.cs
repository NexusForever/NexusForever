using System.IO;
using System.Threading.Tasks;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game;
using NexusForever.WorldServer.Game.Housing;
using NexusForever.WorldServer.Game.Map;

namespace NexusForever.WorldServer.Command.Handler
{
    public class HousingCommandHandler : CommandCategory
    {
        public HousingCommandHandler()
            : base(true, "house")
        {
        }

        [SubCommandHandler("teleport", "[name] - Teleport to a residence, optionally specifying a character")]
        public Task TeleportSubCommandHandler(CommandContext context, string command, string[] parameters)
        {
            string name = parameters.Length == 0 ? context.Session.Player.Name : string.Join(" ", parameters);

            Residence residence = ResidenceManager.Instance.GetResidence(name).GetAwaiter().GetResult();
            if (residence == null)
            {
                if (parameters.Length == 0)
                    residence = ResidenceManager.Instance.CreateResidence(context.Session.Player);
                else
                {
                    context.SendMessageAsync("A residence for that character doesn't exist!");
                    return Task.CompletedTask;
                }
            }

            ResidenceEntrance entrance = ResidenceManager.Instance.GetResidenceEntrance(residence);
            context.Session.Player.TeleportTo(entrance.Entry, entrance.Position, 0u, residence.Id);

            return Task.CompletedTask;
        }

        [SubCommandHandler("decoradd", "decorId [quantity] - Add decor by id to your crate, optionally specifying quantity")]
        public Task DecorAddSubCommandHandler(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length < 1 && parameters.Length > 2)
                return Task.CompletedTask;

            if (!(context.Session.Player.Map is ResidenceMap residenceMap))
            {
                context.SendMessageAsync("You need to be on a housing map to use this command!");
                return Task.CompletedTask;
            }

            uint decorInfoId = uint.Parse(parameters[0]);
            uint quantity    = parameters.Length == 2 ? uint.Parse(parameters[1]) : 1u;

            HousingDecorInfoEntry entry = GameTableManager.Instance.HousingDecorInfo.GetEntry(decorInfoId);
            if (entry == null)
            {
                context.SendMessageAsync($"Invalid decor info id {decorInfoId}!");
                return Task.CompletedTask;
            }

            residenceMap.DecorCreate(entry, quantity);
            return Task.CompletedTask;
        }

        [SubCommandHandler("decorlookup", "name - Returns a list of decor ids that match the supplied name")]
        public Task DecorLookupSubCommandHandler(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length != 1)
                return Task.CompletedTask;

            var sw = new StringWriter();
            sw.WriteLine("Decor Lookup Results:");

            TextTable tt = GameTableManager.Instance.GetTextTable(context.Language);
            foreach (HousingDecorInfoEntry decorEntry in
                SearchManager.Instance.Search<HousingDecorInfoEntry>(parameters[0], context.Language, e => e.LocalizedTextIdName, true))
            {
                string text = tt.GetEntry(decorEntry.LocalizedTextIdName);
                sw.WriteLine($"({decorEntry.Id}) {text}");
            }

            context.SendMessageAsync(sw.ToString());
            return Task.CompletedTask;
        }
    }
}
