using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Costume
{
    [Message(GameMessageOpcode.ClientCostumeItemForget)]
    public class ClientCostumeItemForget : IReadable
    {
        public uint Item2Id { get; private set; }

        public void Read(GamePacketReader reader)
        {
            Item2Id = reader.ReadUInt(18u);
        }
    }
}
