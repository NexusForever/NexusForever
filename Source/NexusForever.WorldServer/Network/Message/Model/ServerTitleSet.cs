using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerTitleSet)]
    class ServerTitleSet : IWritable
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
