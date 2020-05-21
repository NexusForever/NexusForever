using System;

namespace NexusForever.WorldServer.Command
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CommandTargetAttribute : Attribute
    {
        public Type TargetType { get; }

        public CommandTargetAttribute(Type targetType)
        {
            TargetType = targetType;
        }
    }
}
