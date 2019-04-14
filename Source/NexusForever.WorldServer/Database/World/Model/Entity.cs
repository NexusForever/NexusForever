using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.World.Model
{
    public partial class Entity
    {
        public Entity()
        {
            EntityVendorCategory = new HashSet<EntityVendorCategory>();
            EntityVendorItem = new HashSet<EntityVendorItem>();
            EntityStat = new HashSet<EntityStat>();
        }

        public uint Id { get; set; }
        public byte Type { get; set; }
        public uint Creature { get; set; }
        public ushort World { get; set; }
        public ushort Area { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Rx { get; set; }
        public float Ry { get; set; }
        public float Rz { get; set; }
        public uint DisplayInfo { get; set; }
        public ushort OutfitInfo { get; set; }
        public ushort Faction1 { get; set; }
        public ushort Faction2 { get; set; }

        public EntityVendor EntityVendor { get; set; }
        public ICollection<EntityVendorCategory> EntityVendorCategory { get; set; }
        public ICollection<EntityVendorItem> EntityVendorItem { get; set; }
        public ICollection<EntityStat> EntityStat { get; set; }
    }
}
