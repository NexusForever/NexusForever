using System.Xml;

namespace NexusForever.Network.Sts.Model
{
    [Message("/Auth/RequestGameToken")]
    public class RequestGameTokenMessage : IReadable
    {
        public void Read(XmlDocument document)
        {
        }
    }
}
