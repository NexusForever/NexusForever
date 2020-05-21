using System;
using NexusForever.WorldServer.Command.Context;

namespace NexusForever.WorldServer.Command.Convert
{
    public class EnumParameterConverter<T> : IParameterConvert where T : Enum
    {
        public object Convert(ICommandContext context, ParameterQueue queue)
        {
            return Enum.Parse(typeof(T), queue.Dequeue());
        }
    }
}
