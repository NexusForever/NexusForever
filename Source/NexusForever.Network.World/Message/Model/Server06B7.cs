using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.Server06B7)]
    public class Server06B7 : IWritable
    {
        public ushort Unknown0 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0, 14);
        }
    }
}
