using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientQuestRetry)]
    public class ClientQuestRetry : IReadable
    {
        public ushort QuestId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            QuestId = reader.ReadUShort(15u);
        }
    }
}
