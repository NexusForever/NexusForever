using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCinematicShowAnimate)]
    public class ServerCinematicShowAnimate : IWritable
    {
        public uint Delay { get; set; }
        public bool Show { get; set; }
        public bool Animate { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Delay);
            writer.Write(Show);
            writer.Write(Animate);
        }
    }
}
