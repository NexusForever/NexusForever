using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class RealmConfigCustomLocation
    {
        public byte Id { get; set; }
        public uint CustomLocationId { get; set; }
        public float Position0 { get; set; }
        public float Position1 { get; set; }
        public float Position2 { get; set; }
        public float Facing0 { get; set; }
        public float Facing1 { get; set; }
        public float Facing2 { get; set; }
        public float Facing3 { get; set; }
        public ushort WorldId { get; set; }
        public ushort WorldZoneId { get; set; }

        public virtual RealmConfig IdNavigation { get; set; }
    }
}
