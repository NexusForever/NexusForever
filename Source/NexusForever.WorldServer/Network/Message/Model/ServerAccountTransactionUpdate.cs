using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerAccountTransactionUpdate)]
    public class ServerAccountTransactionUpdate : IWritable
    {
        public string TransactionGuid { get; set; }
        public bool Redeemed { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.WriteStringWide(TransactionGuid);
            writer.Write(Redeemed);
        }
    }
}
