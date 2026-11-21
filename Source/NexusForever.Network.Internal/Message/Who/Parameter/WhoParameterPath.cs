using NexusForever.Game.Static.Who;
using Path = NexusForever.Game.Static.PlayerPath.Path;

namespace NexusForever.Network.Internal.Message.Who.Parameter
{
    public class WhoParameterPath : IWhoParameter
    {
        public WhoParameterType Type => WhoParameterType.Path;

        public Path PathId { get; set; }
    }
}
