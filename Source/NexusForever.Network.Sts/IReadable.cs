using System.Xml;

namespace NexusForever.Network.Sts
{
    public interface IReadable
    {
        void Read(XmlDocument document);
    }
}
