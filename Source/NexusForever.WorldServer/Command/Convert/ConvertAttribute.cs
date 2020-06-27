using System;

namespace NexusForever.WorldServer.Command.Convert
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConvertAttribute : Attribute
    {
        public Type Type { get; }

        public ConvertAttribute(Type type)
        {
            Type = type;
        }
    }
}
