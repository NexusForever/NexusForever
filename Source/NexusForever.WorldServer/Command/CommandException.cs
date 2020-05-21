using System;

namespace NexusForever.WorldServer.Command
{
    public class CommandException : Exception
    {
        public CommandException(string message)
            : base(message)
        {
        }
    }
}
