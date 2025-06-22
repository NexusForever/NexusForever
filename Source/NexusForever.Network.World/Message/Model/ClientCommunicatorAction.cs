using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientCommunicatorAction)]
    public class ClientCommunicatorAction : IReadable
    {
        public uint QuestId { get; private set; }
        public bool Engaged { get; private set; } // 1 = Player engaged with communicator, 0 = ignored

        public void Read(GamePacketReader reader)
        {
            QuestId = reader.ReadUInt(15u);
            Engaged = reader.ReadBit();
        }
    }
}
