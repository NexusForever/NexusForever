using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Database.World.Model
{
    public partial class Tutorial
    {
        public uint Id { get; set; }
        public uint Type { get; set; }
        public uint TriggerId { get; set; }
        public string Note { get; set; }
    }
}
