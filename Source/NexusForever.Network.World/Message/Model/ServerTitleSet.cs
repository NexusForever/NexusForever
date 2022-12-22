using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerTitleSet)]
    public class ServerTitleSet : IWritable
    {
        public uint Guid { get; set; }
        public ushort Title { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Guid); 
            writer.Write(Title, 14);
        }
    }
}
