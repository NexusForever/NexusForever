using System.Numerics;
using NexusForever.WorldServer.Command.Context;

namespace NexusForever.WorldServer.Command.Convert
{
    [Convert(typeof(Vector3))]
    public class Vector3ParameterConverter : IParameterConvert
    {
        public object Convert(ICommandContext context, ParameterQueue queue)
        {
            return new Vector3(float.Parse(queue.Dequeue()), float.Parse(queue.Dequeue()), float.Parse(queue.Dequeue()));
        }
    }
}
