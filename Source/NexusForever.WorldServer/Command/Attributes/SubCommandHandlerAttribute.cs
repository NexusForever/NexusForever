using NexusForever.WorldServer.Game.Account.Static;
using System;

namespace NexusForever.WorldServer.Command.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SubCommandHandlerAttribute : Attribute
    {
        public string Command { get; }
        public string HelpText { get; }
        public Permission RequiredPermission { get; }

        public SubCommandHandlerAttribute(string command, string helpText = null, Permission requiredPermission = Permission.Everything)
        {
            Command = command;
            HelpText = helpText;
            RequiredPermission = requiredPermission;
        }
    }
}
