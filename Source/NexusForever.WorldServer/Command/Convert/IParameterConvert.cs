using NexusForever.WorldServer.Command.Context;

namespace NexusForever.WorldServer.Command.Convert
{
    public interface IParameterConvert
    {
        object Convert(ICommandContext context, ParameterQueue queue);
    }
}
