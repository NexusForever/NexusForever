using NexusForever.WorldServer.Game.Guild.Static;
using System;

namespace NexusForever.WorldServer.Game.Guild
{
    [AttributeUsage(AttributeTargets.Method)]
    public class GuildOperationHandlerAttribute : Attribute
    {
        public GuildOperation Operation { get; }

        public GuildOperationHandlerAttribute(GuildOperation operation)
        {
            Operation = operation;
        }
    }
}
