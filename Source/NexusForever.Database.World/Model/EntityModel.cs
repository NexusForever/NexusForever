using System.Collections.Generic;

namespace NexusForever.Database.World.Model
{
    public class EntityModel
    {
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
        public byte QuestChecklistIdx { get; set; }
        public ulong ActivePropId { get; set; }

        public virtual EntitySplineModel Spline { get; set; }
        public virtual EntityVendorModel Vendor { get; set; }
        public virtual ICollection<EntityStatModel> Stats { get; set; }
        public virtual ICollection<EntityVendorCategoryModel> VendorCategories { get; set; }
        public virtual ICollection<EntityVendorItemModel> VendorItems { get; set; }
    }
}
