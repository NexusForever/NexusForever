using NexusForever.Game.Static.Reputation;
using NexusForever.Game.Static.Who;

namespace NexusForever.Network.Internal.Message.Who.Parameter
{
    public class WhoParameterFaction : IWhoParameter
    {
        public WhoParameterType Type => WhoParameterType.Faction;

        public Faction FactionId { get; set; }
    }
}
