using NexusForever.Network.Message;
using NexusForever.Network.World.Entity;

namespace NexusForever.Network.World.Message.Model.Shared
{
    public class TelegraphPosition : IWritable
    {
        public ushort   TelegraphId { get; set; }
        public uint     AttachedUnitId { get; set; }
        public byte     TargetFlags { get; set; }
        public Position Position { get; set; } = new Position();
        public float    Yaw { get; set; }
        public float    Pitch { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(TelegraphId);
            writer.Write(AttachedUnitId);
            writer.Write(TargetFlags);
            Position.Write(writer);
            writer.Write(Yaw);
            writer.Write(Pitch);
        }
    }
}
