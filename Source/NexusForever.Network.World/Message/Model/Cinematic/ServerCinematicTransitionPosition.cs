using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCinematicTransitionPosition)]
    public class ServerCinematicTransitionPosition : IWritable
    {
        public uint Delay { get; set; }
        public Position Position { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Delay);
            Position.Write(writer);
        }
    }
}
