using System.Xml;

namespace NexusForever.StsServer.Network.Message
{
    public interface IReadable
    {
        void Read(XmlDocument document);
    }
}
