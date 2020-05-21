using System;
using NexusForever.WorldServer.Command.Static;

namespace NexusForever.WorldServer.Command
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ParameterAttribute : Attribute
    {
        public string HelpText { get; }
        public ParameterFlags Flags { get; }
        public Type Converter { get; }

        public bool IsOptional => (Flags & ParameterFlags.Optional) != 0;

        public ParameterAttribute(string helpText, ParameterFlags flags = ParameterFlags.None, Type converter = null)
        {
            HelpText  = helpText;
            Flags     = flags;
            Converter = converter;
        }
    }
}
