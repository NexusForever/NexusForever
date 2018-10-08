using System.Collections.Generic;

namespace NexusForever.StsServer.Network.Packet
{
    public abstract class StsPacket
    {
        public string Protocol { get; protected set; }
        public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();
        public string Body { get; protected set; }
    }
}
