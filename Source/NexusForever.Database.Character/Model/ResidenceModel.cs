using System.Collections.Generic;

namespace NexusForever.Database.Character.Model
{
    public class ResidenceModel
    {
        public ulong Id { get; set; }
        public ulong OwnerId { get; set; }
        public byte PropertyInfoId { get; set; }
        public string Name { get; set; }
        public byte PrivacyLevel { get; set; }
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

        public CharacterModel Owner { get; set; }
        public HashSet<ResidenceDecorModel> Decor { get; set; } = new HashSet<ResidenceDecorModel>();
        public HashSet<ResidencePlotModel> Plots { get; set; } = new HashSet<ResidencePlotModel>();
    }
}
