using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Who;

namespace NexusForever.Network.Internal.Message.Who.Parameter
{
    public class WhoParameterRace : IWhoParameter
    {
        public WhoParameterType Type => WhoParameterType.Race;

        public Race RaceId { get; set; }
    }
}
