using NexusForever.WorldServer.Game.Account.Static;
using System;

namespace NexusForever.WorldServer.Command.Attributes
{

    public class NameAttribute : Attribute
    {
        public string Name { get; }
        public Permission PermissionRequired { get; }

        public NameAttribute(string name, Permission permission = Permission.Everything)
        {
            Name = name;
            PermissionRequired = permission;
        }
    }
}
