using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model.Shared
{
    public class AccountCurrency : IWritable
    {
        public byte AccountCurrencyType { get; set; }
        public ulong Amount { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(AccountCurrencyType, 5u);
            writer.Write(Amount);
        }
    }
}
