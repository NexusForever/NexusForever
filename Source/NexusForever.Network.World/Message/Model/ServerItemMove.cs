using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerItemMove)]
    public class ServerItemMove : IWritable
    {
        public ItemDragDrop To { get; set; }

        public void Write(GamePacketWriter writer)
        {
            To.Write(writer);
        }
    }
}
