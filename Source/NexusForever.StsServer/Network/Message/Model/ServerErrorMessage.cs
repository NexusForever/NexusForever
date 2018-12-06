using System.Xml;

namespace NexusForever.StsServer.Network.Message.Model
{
    public class ServerErrorMessage : IWritable
    {
        private int code;

        public ServerErrorMessage(int code)
        {
            this.code = code;
        }

        public void Write(XmlWriter writer)
        {   
            writer.WriteStartElement("Error");

            writer.WriteStartAttribute("code");
            writer.WriteValue(code);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("server");
            writer.WriteValue(0); // TODO
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("module");
            writer.WriteValue(0); // TODO
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("line");
            writer.WriteValue(0); // TODO
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("text");
            writer.WriteValue(0); // TODO
            writer.WriteEndAttribute();

            writer.WriteEndElement();
        }
    }
}
