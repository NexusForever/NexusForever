using System.Xml;

namespace NexusForever.Network.Sts.Model
{
    public class RequestGameTokenResponse : IWritable
    {
        public string Token { get; set; }

        public void Write(XmlWriter writer)
        {
            writer.WriteStartElement("Reply");

            writer.WriteStartElement("Token");
            writer.WriteString(Token);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }
    }
}
