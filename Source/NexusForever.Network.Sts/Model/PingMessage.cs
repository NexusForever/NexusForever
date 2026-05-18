using System.Xml;

namespace NexusForever.Network.Sts.Model
{
    [Message("/Sts/Ping")]
    public class PingMessage : IReadable
    {
        public void Read(XmlDocument document)
        {
        }
    }
}
