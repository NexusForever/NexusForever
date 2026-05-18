using NexusForever.Network.Internal.Static;

namespace NexusForever.Network.Internal.Configuration
{
    public class BrokerConfig
    {
        public BrokerProvider Broker { get; set; }
        public string ConnectionString { get; set; }
        public string InputQueue { get; set; }
    }
}
