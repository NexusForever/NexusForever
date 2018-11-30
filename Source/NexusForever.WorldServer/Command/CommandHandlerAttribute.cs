using System;

namespace NexusForever.WorldServer.Command
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)] // In case of commands like additem/itemadd
    public class CommandHandlerAttribute : Attribute
    {
        public string Command { get; set; }

        public CommandHandlerAttribute(string command)
        {
            Command = command;
        }
    }
}
