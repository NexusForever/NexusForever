using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientQuestSetTracked)]
    public class ClientQuestSetTracked : IReadable
    {
        public ushort QuestId { get; private set; }
        public bool Tracked { get; private set; }

        public void Read(GamePacketReader reader)
        {
            QuestId = reader.ReadUShort(15u);
            Tracked = reader.ReadBit();
        }
    }
}
