using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
{
    [Message(GameMessageOpcode.ClientItemBankOperation)]
    public class ClientItemBankOperation : IReadable
    {
        public ulong ItemGuid { get; private set; }
        public bool ToBank { get; private set; } // 0 = move to inventory, 1 = move to bank

        public void Read(GamePacketReader reader)
        {
            ItemGuid = reader.ReadULong();
            ToBank = reader.ReadBit();
        }
    }
}
