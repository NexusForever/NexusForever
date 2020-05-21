using NexusForever.WorldServer.Command.Context;

namespace NexusForever.WorldServer.Command
{
    public class PendingCommand
    {
        public ICommandContext Context { get; }
        public string CommandText { get; }

        public PendingCommand(ICommandContext context, string commandText)
        {
            Context     = context;
            CommandText = commandText;
        }
    }
}
