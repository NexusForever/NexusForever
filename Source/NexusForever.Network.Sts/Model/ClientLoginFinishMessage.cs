using System.Xml;

namespace NexusForever.Network.Sts.Model
{
    [Message("/Auth/LoginFinish")]
    public class ClientLoginFinishMessage : IReadable
    {
        public void Read(XmlDocument document)
        {
        }
    }
}
