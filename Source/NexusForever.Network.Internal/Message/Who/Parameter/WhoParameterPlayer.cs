using NexusForever.Game.Static.Who;

namespace NexusForever.Network.Internal.Message.Who.Parameter
{
    public class WhoParameterPlayer : IWhoParameter
    {
        public WhoParameterType Type => WhoParameterType.Player;

        public string PlayerName { get; set; }
    }
}
