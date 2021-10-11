using NexusForever.WorldServer.Command.Context;

namespace NexusForever.WorldServer.Command.Convert
{
    public class StringLowerParameterConverter : StringParameterConverter
    {
        public override object Convert(ICommandContext context, ParameterQueue queue)
        {
            return ((string)base.Convert(context, queue)).ToLower();
        }
    }
}
