using NexusForever.Game.Static.Guild;

namespace NexusForever.Game.Guild
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
