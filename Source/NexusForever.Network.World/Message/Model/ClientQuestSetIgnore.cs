using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientQuestSetIgnore)]
    public class ClientQuestSetIgnore : IReadable
    {
        public ushort QuestId { get; private set; }
        public bool Ignore { get; private set; }

        public void Read(GamePacketReader reader)
        {
            QuestId = reader.ReadUShort(15u);
            Ignore  = reader.ReadBit();
        }
    }
}
