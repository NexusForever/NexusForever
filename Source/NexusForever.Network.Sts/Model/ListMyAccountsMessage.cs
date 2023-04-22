using System.Xml;

namespace NexusForever.Network.Sts.Model
{
    [Message("/GameAccount/ListMyAccounts")]
    public class ListMyAccountsMessage : IReadable
    {
        public void Read(XmlDocument document)
        {
        }
    }
}
