using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Cinematic
{
    [Message(GameMessageOpcode.ServerCinematicCameraRotation)]
    public class ServerCinematicCameraRotation : IWritable
    {
        public uint Delay { get; set; }
        public float Yaw { get; set; } // in radians, multiple rotations are normalized to [0, 2pi]
        public float Pitch { get; set; } // in radians, limited to [-pi/2, pi/2]

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Delay);
            writer.Write(Yaw);
            writer.Write(Pitch);
        }
    }
}
