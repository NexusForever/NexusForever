using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientQuestShareResult)]
    public class ClientQuestShareResult : IReadable
    {
        public ushort QuestId { get; private set; }
        public bool Result { get; private set; }

        public void Read(GamePacketReader reader)
        {
            QuestId = reader.ReadUShort(15u);
            Result  = reader.ReadBit();
        }
    }
}
