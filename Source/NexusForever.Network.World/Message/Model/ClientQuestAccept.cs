using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientQuestAccept)]
    public class ClientQuestAccept : IReadable
    {
        public ItemLocation ItemLocation { get; } = new();
        public ushort QuestId { get; private set; }

        public void Read(GamePacketReader reader)
        {
            ItemLocation.Read(reader);
            QuestId = reader.ReadUShort(15u);
        }
    }
}
