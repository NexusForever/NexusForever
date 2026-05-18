using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.Server0237)]
    public class Server0237 : IWritable
    {
        public uint Unknown0 { get; set; } = 0;

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Unknown0);
        }
    }
}
