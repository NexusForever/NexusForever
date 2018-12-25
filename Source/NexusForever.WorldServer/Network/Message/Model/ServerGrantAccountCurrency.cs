using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{

    [Message(GameMessageOpcode.ServerGrantAccountCurrency, MessageDirection.Server)]
    public class ServerGrantAccountCurrency : IWritable
    {
        public byte Type { get; set; }
        public ulong Amount { get; set; }
        public ulong BonusAmount { get; set; } // This is used to say how many of the Amount were granted by a bonus (like Signature status).
        public ulong Unk4 { get; set; }
        public ulong Unk5 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Type, 5);
            writer.Write(Amount);
            writer.Write(BonusAmount);
            writer.Write(Unk4);
            writer.Write(Unk5);
        }
    }
}
