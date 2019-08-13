using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.World.Model
{
    public partial class EntityStats
    {
        public uint Id { get; set; }
        public byte Stat { get; set; }
        public float Value { get; set; }

        public virtual Entity IdNavigation { get; set; }
    }
}
