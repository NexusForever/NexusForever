using System;
using System.Collections.Generic;

namespace NexusForever.Database.Character.Model
{
    public partial class PropertyBaseModel
    {
        public uint Type { get; set; }
        public uint Subtype { get; set; }
        public uint Property { get; set; }
        public ushort ModType { get; set; }
        public float Value { get; set; }
        public string Note { get; set; }
    }
}
