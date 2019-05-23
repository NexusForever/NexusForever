using System.Linq;
using System.Threading.Tasks;
using NexusForever.Shared.GameTable;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game.Account.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Title", Permission.None)]
    public class TitleCommandHandler : CommandCategory
    {
        public TitleCommandHandler()
            : base(true, "title")
        {
        }

        [SubCommandHandler("add", "titleId - Add a title to the character", Permission.CommandTitleAdd)]
        public Task AddTitleSubCommand(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length <= 0)
                return Task.CompletedTask;

            context.Session.Player.TitleManager.AddTitle(ushort.Parse(parameters[0]));
            return Task.CompletedTask;
        }

        [SubCommandHandler("revoke", "titleId - Revoke a title from the character", Permission.CommandTitleRevoke)]
        public Task RemoveTitleSubCommand(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length <= 0)
                return Task.CompletedTask;

            context.Session.Player.TitleManager.RevokeTitle(ushort.Parse(parameters[0]));
            return Task.CompletedTask;
        }

        [SubCommandHandler("all", "Add all titles to the character", Permission.CommandTitleAll)]
        public Task AddAllTitlesSubCommand(CommandContext context, string command, string[] parameters)
        {
            context.Session.Player.TitleManager.AddAllTitles();
            return Task.CompletedTask;
        }

        [SubCommandHandler("none", "Revoke all titles from the character", Permission.CommandTitleNone)]
        public Task RemoveAllTitlesSubCommand(CommandContext context, string command, string[] parameters)
        {
            context.Session.Player.TitleManager.RevokeAllTitles();
            return Task.CompletedTask;
        }
    }
}
