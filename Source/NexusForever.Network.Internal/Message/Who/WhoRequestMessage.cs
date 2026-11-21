using NexusForever.Network.Internal.Message.Shared;
using NexusForever.Network.Internal.Message.Who.Parameter;

namespace NexusForever.Network.Internal.Message.Who
{
    public class WhoRequestMessage
    {
        public Identity Identity { get; set; }
        public List<IWhoParameter> Parameters { get; set; } = [];
        public List<uint> ParameterGroupCounts { get; set; } = [];
    }
}
