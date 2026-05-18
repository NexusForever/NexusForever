using System.Xml;

namespace NexusForever.Network.Sts.Model
{
    public class ServerKeyDataMessage : IWritable
    {
        public string KeyData { get; set; }

        public void Write(XmlWriter writer)
        {
            writer.WriteStartElement("Reply");

            writer.WriteStartElement("KeyData");
            writer.WriteString(KeyData);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }
    }
}
