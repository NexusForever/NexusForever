using NexusForever.Game.Static.Who;

namespace NexusForever.Network.Internal.Message.Who.Parameter
{
    public class WhoParameterLevel : IWhoParameter
    {
        public WhoParameterType Type => WhoParameterType.Level;

        public uint BottomLevel { get; set; }
        public uint TopLevel { get; set; }
    }
}
