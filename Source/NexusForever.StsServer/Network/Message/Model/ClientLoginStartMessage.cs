using System.Xml;

namespace NexusForever.StsServer.Network.Message.Model
{
    [Message("/Auth/LoginStart")]
    public class ClientLoginStartMessage : IReadable
    {
        public string LoginName { get; private set; }
        //public string NetAddress { get; private set; }

        public void Read(XmlDocument document)
        {
            XmlNode rootNode = document["Request"];
            LoginName = rootNode["LoginName"].GetValue<string>();
            //NetAddress = rootNode["NetAddress"].GetValue<string>();
        }
    }
}
