using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ClientQuestAccept)]
    public class ClientQuestAccept : IReadable
    {
        public ItemLocation ItemLocation { get; } = new ItemLocation();
        public ushort QuestId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ItemLocation.Read(reader);
            QuestId = reader.ReadUShort(15u);
        }
    }
}
