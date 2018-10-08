using System.Xml;

namespace NexusForever.StsServer.Network.Message.Model
{
    public class ServerLoginStartMessage : IWritable
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
