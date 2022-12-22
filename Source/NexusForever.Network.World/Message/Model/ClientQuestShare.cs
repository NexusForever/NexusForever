using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientQuestShare)]
    public class ClientQuestShare : IReadable
    {
        public ushort QuestId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            QuestId = reader.ReadUShort(15u);
        }
    }
}
