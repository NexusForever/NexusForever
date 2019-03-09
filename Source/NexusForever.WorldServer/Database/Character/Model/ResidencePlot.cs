using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class ResidencePlot
    {
        public ulong Id { get; set; }
        public byte Index { get; set; }
        public ushort PlotInfoId { get; set; }
        public ushort PlugItemId { get; set; }
        public byte PlugFacing { get; set; }
        public byte BuildState { get; set; }

        public virtual Residence IdNavigation { get; set; }
    }
}
