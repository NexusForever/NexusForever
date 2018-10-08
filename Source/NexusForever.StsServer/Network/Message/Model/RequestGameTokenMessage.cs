using System.Xml;

namespace NexusForever.StsServer.Network.Message.Model
{
    [Message("/Auth/RequestGameToken")]
    public class RequestGameTokenMessage : IReadable
    {
        public void Read(XmlDocument document)
        {
        }
    }
}
