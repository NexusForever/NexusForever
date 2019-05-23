using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game.Account.Static;
using NLog;

namespace NexusForever.WorldServer.Command.Handler
{
    public abstract class CommandHandlerBase : ICommandHandler
    {
        public ILogger Logger { get; } = LogManager.GetCurrentClassLogger();

        public abstract int Order { get; }
        public abstract Permission RequiredPermission { get; }
        public abstract IEnumerable<string> GetCommands();
        public abstract Task HandleAsync(CommandContext session, string text);
        public abstract Task<bool> HandlesAsync(CommandContext session, string input);

        protected static void ParseCommand(string value, out string command, out string[] parameters)
        {
            string[] split = value.Split(' ');
            command = split[0];
            parameters = split.Skip(1).ToArray();
        }
    }
}
