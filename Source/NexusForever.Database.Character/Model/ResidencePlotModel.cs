namespace NexusForever.Database.Character.Model
{
    public class ResidencePlotModel
    {
        public ulong Id { get; set; }
        public byte Index { get; set; }
        public ushort PlotInfoId { get; set; }
        public ushort PlugItemId { get; set; }
        public byte PlugFacing { get; set; }
        public byte BuildState { get; set; }

        public ResidenceModel Residence { get; set; }
    }
}
