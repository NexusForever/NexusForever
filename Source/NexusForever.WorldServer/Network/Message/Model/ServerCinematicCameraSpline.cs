using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerCinematicCameraSpline)]
    public class ServerCinematicCameraSpline : IWritable
    {
        public uint Delay { get; set; }
        public uint Spline { get; set; }
        public uint SplineMode { get; set; }
        public float Speed { get; set; }
        public bool Target { get; set; }
        public bool UseRotation { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Delay);
            writer.Write(Spline);
            writer.Write(SplineMode);
            writer.Write(Speed);
            writer.Write(Target);
            writer.Write(UseRotation);
        }
    }
}
