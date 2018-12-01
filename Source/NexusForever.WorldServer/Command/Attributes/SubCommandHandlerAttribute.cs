using System;

namespace NexusForever.WorldServer.Command.Attributes
{
    public class SubCommandHandlerAttribute : Attribute
    {
        public string Command { get; }
        public string HelpText {get;}
        public SubCommandHandlerAttribute(string command, string helpText = null)
        {
            Command = command;
            HelpText = helpText;
        }
    }
}