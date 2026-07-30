using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Item
{
    [Message(GameMessageOpcode.ServerItemSwap)]
    public class ServerItemSwap : IWritable
    {
        public ItemDragDrop To { get; set; }
        public ItemDragDrop From { get; set; }

        public void Write(GamePacketWriter writer)
        {
            To.Write(writer);
            From.Write(writer);
        }
    }
}
