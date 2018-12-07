using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NexusForever.Shared.GameTable;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Title")]
    public class TitleCommandHandler : CommandCategory
    {
        public TitleCommandHandler()
            : base(true, "title")
        {
        }

        [SubCommandHandler("add", "titleId - Add a title to the character")]
        public Task AddTitleSubCommand(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length <= 0)
                return Task.CompletedTask;

            context.Session.Player.TitleManager.Add(uint.Parse(parameters[0]));
            return Task.CompletedTask;
        }

        [SubCommandHandler("remove", "titleId - Remove a title from the character")]
        public Task RemoveTitleSubCommand(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length <= 0)
                return Task.CompletedTask;

            context.Session.Player.TitleManager.Remove(uint.Parse(parameters[0]));
            return Task.CompletedTask;
        }

        [SubCommandHandler("all", "Add all titles to the character")]
        public Task AddAllTitlesSubCommand(CommandContext context, string command, string[] parameters)
        {
            context.Session.Player.TitleManager.Owned = GameTableManager.CharacterTitle.Entries.Select(entry => (ulong) entry.Id).ToList();
            return Task.CompletedTask;
        }

        [SubCommandHandler("none", "Remove all titles from the character")]
        public Task RemoveAllTitlesSubCommand(CommandContext context, string command, string[] parameters)
        {
            context.Session.Player.TitleManager.Owned = new List<ulong>();
            return Task.CompletedTask;
        }
    }
}
