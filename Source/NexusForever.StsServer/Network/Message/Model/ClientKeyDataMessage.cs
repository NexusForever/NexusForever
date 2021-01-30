using System;
using System.IO;
using System.Xml;

namespace NexusForever.StsServer.Network.Message.Model
{
    [Message("/Auth/KeyData")]
    public class ClientKeyDataMessage : IReadable
    {
        public byte[] A { get; private set; }
        public byte[] M1 { get; private set; }

        public void Read(XmlDocument document)
        {
            XmlNode rootNode = document["Request"];
            string keyData = rootNode["KeyData"].GetValue<string>();

            using var stream = new MemoryStream(Convert.FromBase64String(keyData));
            using var reader = new BinaryReader(stream);
            int lengthA = reader.ReadInt32();
            A = reader.ReadBytes(lengthA);
            int lengthM = reader.ReadInt32();
            M1 = reader.ReadBytes(lengthM);
        }
    }
}
