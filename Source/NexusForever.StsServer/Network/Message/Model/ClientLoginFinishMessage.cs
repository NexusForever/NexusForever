using System.Xml;

namespace NexusForever.StsServer.Network.Message.Model
{
    [Message("/Auth/LoginFinish")]
    public class ClientLoginFinishMessage : IReadable
    {
        public void Read(XmlDocument document)
        {
        }
    }
}
