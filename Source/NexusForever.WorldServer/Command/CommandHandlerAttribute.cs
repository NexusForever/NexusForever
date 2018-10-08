using System;

namespace NexusForever.WorldServer.Command
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandHandlerAttribute : Attribute
    {
        public string Command { get; set; }

        public CommandHandlerAttribute(string command)
        {
            Command = command;
        }
    }
}
