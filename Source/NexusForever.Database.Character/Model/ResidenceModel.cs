using System.Collections.Generic;
using NexusForever.Game.Static.Housing;

namespace NexusForever.Database.Character.Model
{
    public class ResidenceModel
    {
        public ulong Id { get; set; }
        public ulong? OwnerId { get; set; }
        public ulong? GuildOwnerId { get; set; }
        public PropertyInfoId PropertyInfoId { get; set; }
        public string Name { get; set; }
        public ResidencePrivacyLevel PrivacyLevel { get; set; }
        public ushort WallpaperId { get; set; }
        public ushort RoofDecorInfoId { get; set; }
        public ushort EntrywayDecorInfoId { get; set; }
        public ushort DoorDecorInfoId { get; set; }
        public ushort GroundWallpaperId { get; set; }
        public ushort MusicId { get; set; }
        public ushort SkyWallpaperId { get; set; }
        public ResidenceFlags Flags { get; set; }
        public byte ResourceSharing { get; set; }
        public byte GardenSharing { get; set; }

        public CharacterModel Character { get; set; }
        public GuildModel Guild { get; set; }
        public ICollection<ResidenceDecor> Decor { get; set; } = new HashSet<ResidenceDecor>();
        public ICollection<ResidencePlotModel> Plot { get; set; } = new HashSet<ResidencePlotModel>();
    }
}
