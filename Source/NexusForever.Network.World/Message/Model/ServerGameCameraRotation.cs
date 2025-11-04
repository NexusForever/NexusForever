using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerGameCameraRotation)]
    public class ServerGameCameraRotation : IWritable
    {
        public float Yaw { get; set; } // in radians
        public float Pitch { get; set; } // in radians

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Yaw);
            writer.Write(Pitch);
        }
    }
}
