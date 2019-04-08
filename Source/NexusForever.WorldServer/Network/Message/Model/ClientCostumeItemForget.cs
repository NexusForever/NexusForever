using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientCostumeItemForget)]
    public class ClientCostumeItemForget : IReadable
    {
        public uint ItemId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ItemId = reader.ReadUInt(18u);
        }
    }
}
