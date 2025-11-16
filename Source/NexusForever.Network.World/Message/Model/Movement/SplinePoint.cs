using NexusForever.Network.Message;
using System.Numerics;

namespace NexusForever.Network.World.Message.Model.Movement
{
    public class SplinePoint : IWritable
    {
        public uint FrameTime { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Facing { get; set; }
        public float FovY { get; set; }
        public uint EventId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(FrameTime);
            writer.WriteVector3(Position);
            writer.Write(Facing.X);
            writer.Write(Facing.Y);
            writer.Write(Facing.Z);
            writer.Write(Facing.W);
            writer.Write(FovY);
            writer.Write(EventId);
        }
    }
}
