using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NexusForever.WorldServer.Command.Contexts;

namespace NexusForever.WorldServer.Command.Handler
{
    public abstract class CommandHandlerBase : ICommandHandler
    {
        public ILogger Logger { get; }

        protected CommandHandlerBase(ILogger logger)
        {
            Logger = logger;
        }

        public abstract int Order { get; }

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
