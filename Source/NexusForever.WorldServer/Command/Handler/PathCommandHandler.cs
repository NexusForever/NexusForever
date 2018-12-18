using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;
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
        public Task AddPathSubCommand(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length <= 0)
                return Task.CompletedTask;

            uint newPath = 0;
            if (parameters.Length > 0)
                newPath = uint.Parse(parameters[0]);
            log.Info($"Received {command}: {parameters}, {newPath}");

            context.Session.Player.Path.ActivePath = (Path)newPath;

            context.Session.EnqueueMessageEncrypted(new ServerSetUnitPathType
            {
                Guid = context.Session.Player.Guid,
                Path = context.Session.Player.Path.ActivePath,
            });
            context.Session.EnqueueMessageEncrypted(new ServerPathLog
            {
                ActivePath = context.Session.Player.Path.ActivePath,
                PathProgress = new ServerPathLog.Progress
                {
                    Soldier = context.Session.Player.Path.SoldierXp,
                    Settler = context.Session.Player.Path.SettlerXp,
                    Scientist = context.Session.Player.Path.ScientistXp,
                    Explorer = context.Session.Player.Path.ExplorerXp
                },
                UnlockedPathMask = context.Session.Player.Path.PathsUnlocked
            });

            return Task.CompletedTask;
        }
    }
}
