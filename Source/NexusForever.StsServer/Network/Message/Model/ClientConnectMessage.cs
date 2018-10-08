using System.Xml;

namespace NexusForever.StsServer.Network.Message.Model
{
    [Message("/Sts/Connect")]
    public class ClientConnectMessage : IReadable
    {
        public uint ConnType { get; private set; }
        public string Address { get; }
        public uint? ConnProductType { get; }
        public uint? ConnAppIndex { get; }
        public uint? ConnDeployment { get; }
        public uint? ConnEpoch { get; }
        public uint ProductType { get; }
        public uint AppIndex { get; }
        public uint? Deployment { get; }
        public uint Epoch { get; }
        public uint Program { get; }
        public uint Build { get; }
        public uint Process { get; }
        public uint? NotifyFlags { get; }
        public uint? VersionFlags { get; }

        public void Read(XmlDocument document)
        {
            XmlNode rootNode = document["Connect"];
            ConnType = rootNode["ConnType"].GetValue<uint>();
        }
    }
}
