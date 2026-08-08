using NexusForever.Network.Message;
using System.Numerics;

namespace NexusForever.Network.World.Message.Model.Movement
{
    // Can modify either a live or static spline
    // Live splines come from ServerSplineAdd, static splines come from the spline2 game table
    [Message(GameMessageOpcode.ServerSplineModify)]
    public class ServerSplineModify : IWritable
    {
        public uint StaticSpline2Id { get; set; }
        public uint LiveSpline2Id { get; set; }
        public Vector3 Offset { get; set; }
        public List<float> YCoordinateOffsets { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(StaticSpline2Id);
            writer.Write(LiveSpline2Id);
            writer.WriteVector3(Offset);
            writer.Write(YCoordinateOffsets.Count);
            YCoordinateOffsets.ForEach(offset => writer.Write(offset));
        }
    }
}
