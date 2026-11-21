using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Cinematic
{
    [Message(GameMessageOpcode.ServerCinematicActorSpline)]
    public class ServerCinematicActorSpline : IWritable
    {
        public uint Delay { get; set; }
        public uint UnitId { get; set; }
        public uint SplineId { get; set; }
        public float SplineSpeed { get; set; }
        public uint SplineMode { get; set; }
        public bool UseRotation { get; set; }
        public bool Strafe { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Delay);
            writer.Write(UnitId);
            writer.Write(SplineId);
            writer.Write(SplineSpeed);
            writer.Write(SplineMode);
            writer.Write(UseRotation);
            writer.Write(Strafe);
        }
    }
}
