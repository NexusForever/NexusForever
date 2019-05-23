using Microsoft.Extensions.Configuration;
using NexusForever.Shared.Configuration;
using NexusForever.WorldServer.Game.Account.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static NexusForever.WorldServer.Command.Handler.CommandCategory;

namespace NexusForever.WorldServer.Command.Handler
{
    public class SubCommandInstance
    {
        public Permission RequiredPermission { get; }
        public SubCommandHandler Handler { get; }

        public SubCommandInstance(SubCommandHandler handler, Permission requiredPermission = Permission.Everything)
        {
            Handler = handler;
            RequiredPermission = requiredPermission;
        }
    }
}
