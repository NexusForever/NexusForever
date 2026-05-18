using System;
using NexusForever.WorldServer.Command.Context;

namespace NexusForever.WorldServer.Command.Convert
{
    [Convert(typeof(TimeSpan))]
    public class TimeSpanParameterConverter : IParameterConvert
    {
        public object Convert(ICommandContext context, ParameterQueue queue)
        {
            return TimeSpan.Parse(queue.Dequeue());
        }
    }
}
