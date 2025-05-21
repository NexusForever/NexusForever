using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientQuestShareResponse)]
    public class ClientQuestShareResponse : IReadable
    {
        public ushort QuestId { get; private set; }
        public bool Accept { get; private set; }

        public void Read(GamePacketReader reader)
        {
            QuestId = reader.ReadUShort(15u);
            Accept  = reader.ReadBit();
        }
    }
}
