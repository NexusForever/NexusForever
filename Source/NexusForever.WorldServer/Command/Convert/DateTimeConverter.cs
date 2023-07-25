using System;
using NexusForever.WorldServer.Command.Context;

namespace NexusForever.WorldServer.Command.Convert
{
    [Convert(typeof(DateTime))]
    public class DateTimeConverter : IParameterConvert
    {
        public object Convert(ICommandContext context, ParameterQueue queue)
        {
            return DateTime.Parse(queue.Dequeue());
        }
    }
}
