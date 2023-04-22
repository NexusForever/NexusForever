using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
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
