using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
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
