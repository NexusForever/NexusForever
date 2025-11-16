using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Movement
{
    // Adds spline to the live Spline set, separate from the static spline set loaded from tables
    [Message(GameMessageOpcode.ServerSplineAdd)]
    public class ServerSplineAdd : IWritable
    {
        public uint Spline2Id { get; set; }
        public uint WorldId { get; set; }
        public byte SplineType { get; set; }
        public List<SplinePoint> SplinePoints { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Spline2Id);
            writer.Write(WorldId);
            writer.Write(SplineType, 2u);
            writer.Write(SplinePoints.Count);
            SplinePoints.ForEach(point => point.Write(writer));
        }
    }
}
