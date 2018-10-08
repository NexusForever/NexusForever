using System.Xml;

namespace NexusForever.StsServer.Network.Message.Model
{
    public class ServerLoginFinishMessage : IWritable
    {
        public string LocationId { get; set; }
        public string UserId { get; set; }
        public uint UserCenter { get; set; }
        public string UserName { get; set; }
        public long AccessMask { get; set; }

        // Status
        // Aliases
        // ExternalAccount
        // PcCafe

        public void Write(XmlWriter writer)
        {
            writer.WriteStartElement("Reply");

            writer.WriteStartElement("LocationId");
            writer.WriteString(LocationId);
            writer.WriteEndElement();

            writer.WriteStartElement("UserId");
            writer.WriteString(UserId);
            writer.WriteEndElement();

            writer.WriteStartElement("UserCenter");
            writer.WriteValue(UserCenter);
            writer.WriteEndElement();

            writer.WriteStartElement("UserName");
            writer.WriteString(UserName);
            writer.WriteEndElement();

            writer.WriteStartElement("AccessMask");
            writer.WriteValue(AccessMask);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }
    }
}
