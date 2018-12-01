using System;

namespace NexusForever.WorldServer.Command.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SubCommandHandlerAttribute : Attribute
    {
        public string Command { get; }
        public string HelpText { get; }
        public SubCommandHandlerAttribute(string command, string helpText = null)
        {
            Command = command;
            HelpText = helpText;
        }
    }
}