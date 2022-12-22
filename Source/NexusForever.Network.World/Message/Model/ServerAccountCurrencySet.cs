using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
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
