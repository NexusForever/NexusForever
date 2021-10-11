using System.Text;
using NexusForever.WorldServer.Command.Context;

namespace NexusForever.WorldServer.Command.Convert
{
    [Convert(typeof(string))]
    public class StringParameterConverter : IParameterConvert
    {
        public virtual object Convert(ICommandContext context, ParameterQueue queue)
        {
            string parameter = queue.Dequeue();
            if (!parameter.StartsWith('"'))
                return parameter;

            // concat parameters between quotes
            // "this will be passed as a single parameter"
            var sb = new StringBuilder();
            parameter = parameter[1..];

            while (true)
            {
                if (parameter.EndsWith('"'))
                {
                    sb.Append(parameter[..^1]);
                    break;
                }

                sb.Append(parameter);
                if (queue.Count == 0)
                    break;

                sb.Append(' ');
                parameter = queue.Dequeue();
            }

            return sb.ToString();
        }
    }
}
