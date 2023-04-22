using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientItemMoveToSupplySatchel)]
    public class ClientItemMoveToSupplySatchel : IReadable
    {
        public ulong ItemGuid { get; private set; }
        public uint Amount { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ItemGuid = reader.ReadULong();
            Amount = reader.ReadUInt();
        }
    }
}
