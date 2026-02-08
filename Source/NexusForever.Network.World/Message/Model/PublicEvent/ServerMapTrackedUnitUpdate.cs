using NexusForever.Network.Message;
using System.Numerics;

namespace NexusForever.Network.World.Message.Model.PublicEvent
{
    [Message(GameMessageOpcode.ServerMapTrackedUnitUpdate)]
    public class ServerMapTrackedUnitUpdate : IWritable
    {
        public uint TrackedUnitId { get; set; }
        public Vector3 Position { get; set; }
        public uint PublicEventObjectiveId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TrackedUnitId);
            writer.Write(Position.X);
            writer.Write(Position.Y);
            writer.Write(Position.Z);
            writer.Write(PublicEventObjectiveId, 14u);
        }
    }
}
