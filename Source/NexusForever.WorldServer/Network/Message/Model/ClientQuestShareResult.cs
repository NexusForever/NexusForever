using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
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
