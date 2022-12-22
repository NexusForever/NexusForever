using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
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
