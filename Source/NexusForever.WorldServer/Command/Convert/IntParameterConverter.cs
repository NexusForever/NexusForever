using NexusForever.WorldServer.Command.Context;

namespace NexusForever.WorldServer.Command.Convert
{
    [Convert(typeof(int))]
    public class IntParameterConverter : IParameterConvert
    {
        public object Convert(ICommandContext context, ParameterQueue queue)
        {
            return int.Parse(queue.Dequeue());
        }
    }
}
