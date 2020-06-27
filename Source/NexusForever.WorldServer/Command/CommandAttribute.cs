using System;
using NexusForever.WorldServer.Game.RBAC.Static;

namespace NexusForever.WorldServer.Command
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        public Permission Permission { get; }
        public string HelpText { get; }
        public string[] Commands { get; }

        public CommandAttribute(Permission permission = Permission.None, string helpText = "", params string[] commands)
        {
            Permission = permission;
            HelpText   = helpText;
            Commands   = commands;
        }
    }
}
