using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ClientCostumeItemUnlock)]
    public class ClientCostumeItemUnlock : IReadable
    {
        public ItemLocation Location { get; } = new();

        public void Read(GamePacketReader reader)
        {
            Location.Read(reader);
        }
    }
}
