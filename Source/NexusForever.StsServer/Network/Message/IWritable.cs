using System.Xml;

namespace NexusForever.StsServer.Network.Message
{
    public interface IWritable
    {
        void Write(XmlWriter writer);
    }
}
