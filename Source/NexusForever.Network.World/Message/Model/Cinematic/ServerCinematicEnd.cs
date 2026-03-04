using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Cinematic
{
    // Starts the outro transition of the cinematic, returning to the game camera
    [Message(GameMessageOpcode.ServerCinematicEnd)]
    public class ServerCinematicEnd : IWritable
    {
        public bool SetGameCamera { get; set; }
        public float Yaw { get; set; } // in radians, multiple rotations are normalized to [0, 2pi]
        public float Pitch { get; set; } // in radians, limited to [-pi/2, pi/2]

        public void Write(GamePacketWriter writer)
        {
            writer.Write(SetGameCamera);
            writer.Write(Yaw);
            writer.Write(Pitch);
        }
    }
}
