namespace NexusForever.Database.Character.Model
{
    public class ResidenceDecor
    {
        public ulong Id { get; set; }
        public ulong DecorId { get; set; }
        public uint DecorInfoId { get; set; }
        public uint DecorType { get; set; }
        public uint PlotIndex { get; set; }
        public float Scale { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Qx { get; set; }
        public float Qy { get; set; }
        public float Qz { get; set; }
        public float Qw { get; set; }
        public ulong DecorParentId { get; set; }
        public ushort ColourShiftId { get; set; }

        public ResidenceModel Residence { get; set; }
    }
}
