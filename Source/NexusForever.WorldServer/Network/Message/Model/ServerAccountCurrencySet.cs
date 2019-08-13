using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerAccountCurrencySet)]
    public class ServerAccountCurrencySet : IWritable
    {
        public List<AccountCurrency> AccountCurrencies { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AccountCurrencies.Count, 32u);

            AccountCurrencies.ForEach(a => a.Write(writer));
        }
    }
}
