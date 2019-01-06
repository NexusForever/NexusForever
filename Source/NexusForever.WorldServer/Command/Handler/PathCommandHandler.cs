using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game.Entity.Static;
using NLog;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Path")]
    public class PathCommandHandler : CommandCategory
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public PathCommandHandler()
            : base(true, "path")
        {
        }

        [SubCommandHandler("activate", "pathId - Activate a path for this player.")]
        public Task AddPathActivateSubCommand(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length <= 0)
                return Task.CompletedTask;

            uint newPath = 0;
            if (parameters.Length > 0)
                newPath = uint.Parse(parameters[0]);

            context.Session.Player.PathManager.ActivatePath((Path)newPath);

            return Task.CompletedTask;
        }

        [SubCommandHandler("unlock", "pathId - Unlock a path for this player.")]
        public Task AddPathUnlockSubCommand(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length > 0)
            {
                uint unlockPath = uint.Parse(parameters[0]);
                context.Session.Player.PathManager.UnlockPath((Path)unlockPath);
            }

            return Task.CompletedTask;
        }

        [SubCommandHandler("addxp", "xp - Add the XP value to the player's curent Path XP.")]
        public Task AddPathAddXPSubCommand(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length > 0)
            {
                uint xp = uint.Parse(parameters[0]);
                context.Session.Player.PathManager.AddXp(xp);
            }

            return Task.CompletedTask;
        }
    }
}
