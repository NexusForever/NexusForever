using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
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
