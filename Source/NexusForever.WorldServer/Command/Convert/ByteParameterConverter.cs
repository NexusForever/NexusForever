using NexusForever.WorldServer.Command.Context;

namespace NexusForever.WorldServer.Command.Convert
{
    [Convert(typeof(byte))]
    public class ByteParameterConverter : IParameterConvert
    {
        public object Convert(ICommandContext context, ParameterQueue queue)
        {
            return byte.Parse(queue.Dequeue());
        }
    }
}
