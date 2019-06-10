using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
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
