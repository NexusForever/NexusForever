using System.Collections.Generic;
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
        public Task AddPathActivateSubCommand(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length <= 0)
                return Task.CompletedTask;

            uint newPath = 0;
            if (parameters.Length > 0)
                newPath = uint.Parse(parameters[0]);

            context.Session.Player.PathManager.ActivatePath((Path)newPath);

            context.Session.Player.PathManager.SendSetUnitPathTypePacket();
            context.Session.Player.PathManager.SendPathLogPacket();

            return Task.CompletedTask;
        }

        [SubCommandHandler("cancelActivationTest", "Used to simulate cancelling an activation request from client")]
        public Task AddPathCancelActivationTestSubCommand(CommandContext context, string command, string[] parameters)
        {
            byte reason = 1;
            if (parameters.Length > 0)
                reason = byte.Parse(parameters[0]);

            context.Session.EnqueueMessageEncrypted(new ServerPathCancelActivate
            {
                Reason = reason
            });
            return Task.CompletedTask;
        }

        [SubCommandHandler("test", "Used to simulate cancelling an activation request from client")]
        public Task AddPathTestSubCommand(CommandContext context, string command, string[] parameters)
        {
            context.Session.EnqueueMessageEncrypted(new Server06B5
            {
                Unknown0 = 9,
                UnknownStructures = new List<Server06B5.UnknownStructure>{
                    new Server06B5.UnknownStructure
                    {
                        Unknown0 = 35,
                        Unknown1 = false,
                        Unknown2 = 0,
                        Unknown3 = 0
                    }
                }
            });
            context.Session.EnqueueMessageEncrypted(new Server06BA
            {
                UnknownStructures = new List<Server06BA.UnknownStructure>{
                    new Server06BA.UnknownStructure
                    {
                        Unknown0 = 35,
                        Unknown1 = false,
                        Unknown2 = 0,
                        Unknown3 = 0,
                        Unknown4 = 1,
                        Unknown5 = 0
                    }
                }
            });
            return Task.CompletedTask;
        }
    }
}
