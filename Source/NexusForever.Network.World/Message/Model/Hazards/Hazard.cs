using NexusForever.Game.Static.Hazards;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Hazards
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
        public bool bUnitBased { get; set; }
        public bool bStartsFull { get; set; }
        public bool bEnabled { get; set; }
        public bool bSuspended { get; set; }
        public bool bDoNotRefill { get; set; }

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
            writer.Write(bUnitBased);
            writer.Write(bStartsFull);
            writer.Write(bEnabled);
            writer.Write(bSuspended);
            writer.Write(bDoNotRefill);
        }
    }
}
