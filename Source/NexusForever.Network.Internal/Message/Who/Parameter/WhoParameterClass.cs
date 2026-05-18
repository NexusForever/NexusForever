using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Who;

namespace NexusForever.Network.Internal.Message.Who.Parameter
{
    public class WhoParameterClass : IWhoParameter
    {
        public WhoParameterType Type => WhoParameterType.Class;

        public Class ClassId { get; set; }
    }
}
