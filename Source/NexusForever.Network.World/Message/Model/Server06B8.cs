using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.Server06B8)]
    public class Server06B8 : IWritable
    {
        public uint Unknown0 { get; set; }
        public ushort Unknown1 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0, 17);
            writer.Write(Unknown1, 14);
        }
    }
}
