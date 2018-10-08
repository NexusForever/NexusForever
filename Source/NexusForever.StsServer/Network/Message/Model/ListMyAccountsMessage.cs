using System.Xml;

namespace NexusForever.StsServer.Network.Message.Model
{
    [Message("/GameAccount/ListMyAccounts")]
    public class ListMyAccountsMessage : IReadable
    {
        public void Read(XmlDocument document)
        {
        }
    }
}
