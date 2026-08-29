using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.AccountInventory
{
    [Message(GameMessageOpcode.ServerAccountCurrencies)]
    public class ServerAccountCurrencies : IWritable
    {
        public List<AccountCurrency> AccountCurrencies { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AccountCurrencies.Count, 32u);

            AccountCurrencies.ForEach(a => a.Write(writer));
        }
    }
}
