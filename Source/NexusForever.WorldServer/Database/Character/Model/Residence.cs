using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class Residence
    {
        public Residence()
        {
            ResidenceDecor = new HashSet<ResidenceDecor>();
            ResidencePlot = new HashSet<ResidencePlot>();
        }

        public ulong Id { get; set; }
        public ulong OwnerId { get; set; }
        public byte PropertyInfoId { get; set; }
        public string Name { get; set; }
        public byte PrivacyLevel { get; set; }
        public ushort ResidenceInfoId { get; set; }
        public ushort WallpaperId { get; set; }
        public ushort RoofDecorInfoId { get; set; }
        public ushort EntrywayDecorInfoId { get; set; }
        public ushort DoorDecorInfoId { get; set; }
        public ushort GroundWallpaperId { get; set; }
        public ushort MusicId { get; set; }
        public ushort SkyWallpaperId { get; set; }
        public ushort Flags { get; set; }
        public byte ResourceSharing { get; set; }
        public byte GardenSharing { get; set; }

        public virtual Character Owner { get; set; }
        public virtual ICollection<ResidenceDecor> ResidenceDecor { get; set; }
        public virtual ICollection<ResidencePlot> ResidencePlot { get; set; }
    }
}
