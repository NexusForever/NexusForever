using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.World.Model
{
    public partial class Disable
    {
        public byte Type { get; set; }
        public uint ObjectId { get; set; }
        public string Note { get; set; }
    }
}
