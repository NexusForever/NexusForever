using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.Server03C0)]
    public class Server03C0 : IWritable
    {
        public byte Unknown0 { get; set; }
        public byte[] Unknown1 { get; set; } = new byte[8];
        public byte[] Unknown2 { get; set; } = new byte[4];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0);
            writer.WriteBytes(Unknown1);
            writer.WriteBytes(Unknown2);
        }
    }
}
