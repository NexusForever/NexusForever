using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;
using System.Numerics;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerCinematicCameraPosVel)]
    public class ServerCinematicCameraPosVel : IWritable
    {
        public uint Delay { get; set; }
        public Position Position { get; set; }
        public Vector3 Velocity { get; set; }
        public bool Target { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Delay);
            Position.Write(writer);
            writer.Write(Velocity.X);
            writer.Write(Velocity.Y);
            writer.Write(Velocity.Z);
            writer.Write(Target);
        }
    }
}

