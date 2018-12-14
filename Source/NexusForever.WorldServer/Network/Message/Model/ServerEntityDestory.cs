using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerEntityDestory, MessageDirection.Server)]
    public class ServerEntityDestory : IWritable
    {
        public uint Guid { get; set; }
        public bool Unknown0 { get; set; }
        public byte Unknown1 { get; set; }
        public uint Unknown2 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Guid);
            writer.Write(Unknown0);
            writer.Write(Unknown1, 5);
            writer.Write(Unknown2);
        }
    }
}
