using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerAccountCurrencyGrant)]
    public class ServerGrantAccountCurrency : IWritable
    {
        public byte Type { get; set; }
        public ulong TotalAmount { get; set; } // This is the new total of chosen currency. Subtract or Adding currency should send a new total.
        public ulong BonusAmount { get; set; } // This is used to say how many of the TotalAmount were granted by signature bonus on this "transaction".
        public ulong Unk4 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Type, 5);
            writer.Write(TotalAmount);
            writer.Write(BonusAmount);
            writer.Write(Unk4);
        }
    }
}
