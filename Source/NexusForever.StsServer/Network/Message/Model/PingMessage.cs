using System.Xml;

namespace NexusForever.StsServer.Network.Message.Model
{
    [Message("/Sts/Ping")]
    public class PingMessage : IReadable
    {
        public void Read(XmlDocument document)
        {
        }
    }
}
