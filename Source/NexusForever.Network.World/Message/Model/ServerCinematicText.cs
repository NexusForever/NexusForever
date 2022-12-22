using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCinematicText)]
    public class ServerCinematicText : IWritable
    {
        public uint Delay { get; set; }
        public uint TextId { get; set; } // 21
        public bool Unknown0 { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Delay);
            writer.Write(TextId);
            writer.Write(Unknown0);
        }
    }
}
