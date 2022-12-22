using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.Server06B6)]
    public class Server06B6 : IWritable
    {
        public ushort Unknown0 { get; set; }
        public uint Unknown1 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0, 14);
            writer.Write(Unknown1);
        }
    }
}
