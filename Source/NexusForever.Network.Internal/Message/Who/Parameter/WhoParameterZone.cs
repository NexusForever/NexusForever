using NexusForever.Game.Static.Who;

namespace NexusForever.Network.Internal.Message.Who.Parameter
{
    public class WhoParameterZone : IWhoParameter
    {
        public WhoParameterType Type => WhoParameterType.Zone;

        public ushort WorldZoneId { get; set; }
    }
}
