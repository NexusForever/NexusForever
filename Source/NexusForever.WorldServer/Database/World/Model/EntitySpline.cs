using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.World.Model
{
    public partial class EntitySpline
    {
        public uint Id { get; set; }
        public ushort SplineId { get; set; }
        public byte Mode { get; set; }
        public float Speed { get; set; }
        public float Fx { get; set; }
        public float Fy { get; set; }
        public float Fz { get; set; }

        public virtual Entity IdNavigation { get; set; }
    }
}
