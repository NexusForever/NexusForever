using System.Xml;

namespace NexusForever.Network.Sts
{
    public interface IWritable
    {
        void Write(XmlWriter writer);
    }
}
