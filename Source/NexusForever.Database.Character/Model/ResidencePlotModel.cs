using NexusForever.Game.Static.Housing;

namespace NexusForever.Database.Character.Model
{
    public class ResidencePlotModel
    {
        public ulong Id { get; set; }
        public byte Index { get; set; }
        public ushort PlotInfoId { get; set; }
        public ushort PlugItemId { get; set; }
        public HousingPlugFacing PlugFacing { get; set; }
        public BuildState BuildState { get; set; }

        public ResidenceModel Residence { get; set; }
    }
}
