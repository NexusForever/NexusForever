using NexusForever.Game.Static.Hazard;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Hazard
{
    public class Hazard : IWritable
    {
        public ushort HazardId { get; set; }
        public HazardType Type { get; set; }
        public float MeterValue { get; set; }
        public float MaxValue { get; set; }
        public uint CurrentThreshold { get; set; }
        public uint ProcSpell4Id { get; set; }
        public uint HazardUnitId { get; set; }
        public uint PulseTimeLeft { get; set; }
        public bool UnitBased { get; set; }
        public bool StartsFull { get; set; }
        public bool Enabled { get; set; }
        public bool Suspended { get; set; }
        public bool DoNotRefill { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(HazardId, 14u);
            writer.Write(Type, 3u);
            writer.Write(MeterValue);
            writer.Write(MaxValue);
            writer.Write(CurrentThreshold, 32u);
            writer.Write(ProcSpell4Id, 18u);
            writer.Write(HazardUnitId);
            writer.Write(PulseTimeLeft);
            writer.Write(UnitBased);
            writer.Write(StartsFull);
            writer.Write(Enabled);
            writer.Write(Suspended);
            writer.Write(DoNotRefill);
        }
    }
}
