using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
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
