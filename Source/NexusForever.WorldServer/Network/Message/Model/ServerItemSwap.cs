using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
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
