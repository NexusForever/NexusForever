using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientQuestComplete)]
    public class ClientQuestComplete : IReadable
    {
        public ushort QuestId { get; private set; }
        public ushort RewardSelection { get; private set; }
        public bool IsCommunique { get; private set; }

        public void Read(GamePacketReader reader)
        {
            QuestId         = reader.ReadUShort(15u);
            RewardSelection = reader.ReadUShort(15u);
            IsCommunique    = reader.ReadBit();
        }
    }
}
