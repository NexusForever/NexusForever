using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Guild;
using NexusForever.WorldServer.Game.Guild.Static;
using NexusForever.WorldServer.Game.Housing.Static;
using NexusForever.WorldServer.Game.Map;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Housing
{
    public class Residence : ISaveCharacter, IBuildable<ServerHousingProperties.Residence>
    {
        public ulong Id { get; }
        public ResidenceType Type { get; }
        public ulong? OwnerId { get; }

        public ulong? GuildOwnerId
        {
            get => guildOwnerId;
            set
            {
                guildOwnerId = value;
                saveMask |= ResidenceSaveMask.GuildOwner;
            }
        }

        private ulong? guildOwnerId;

        public PropertyInfoId PropertyInfoId
        {
            get => propertyInfoId;
            set
            {
                propertyInfoId = value;
                saveMask |= ResidenceSaveMask.PropertyInfo;

                UpdatePlots();
            }
        }

        private PropertyInfoId propertyInfoId;

        public string Name
        {
            get => name;
            set
            {
                name = value;
                saveMask |= ResidenceSaveMask.Name;
            }
        }

        private string name;

        public ResidencePrivacyLevel PrivacyLevel
        {
            get => privacyLevel;
            set
            {
                if (value > ResidencePrivacyLevel.Private)
                    throw new ArgumentOutOfRangeException();

                privacyLevel = value;
                saveMask |= ResidenceSaveMask.PrivacyLevel;
            }
        }

        private ResidencePrivacyLevel privacyLevel;

        public ushort Wallpaper
        {
            get => wallpaperId;
            set
            {
                if (GameTableManager.Instance.HousingWallpaperInfo.GetEntry(value) == null)
                    throw new ArgumentOutOfRangeException();

                wallpaperId = value;
                saveMask |= ResidenceSaveMask.Wallpaper;
            }
        }

        private ushort wallpaperId;

        public ushort Roof
        {
            get => roofDecorInfoId;
            set
            {
                if (GameTableManager.Instance.HousingDecorInfo.GetEntry(value) == null)
                    throw new ArgumentOutOfRangeException();

                roofDecorInfoId = value;
                saveMask |= ResidenceSaveMask.Roof;
            }
        }

        private ushort roofDecorInfoId;

        public ushort Entryway
        {
            get => entrywayDecorInfoId;
            set
            {
                if (GameTableManager.Instance.HousingDecorInfo.GetEntry(value) == null)
                    throw new ArgumentOutOfRangeException();

                entrywayDecorInfoId = value;
                saveMask |= ResidenceSaveMask.Entryway;
            }
        }

        private ushort entrywayDecorInfoId;

        public ushort Door
        {
            get => doorDecorInfoId;
            set
            {
                if (GameTableManager.Instance.HousingDecorInfo.GetEntry(value) == null)
                    throw new ArgumentOutOfRangeException();

                doorDecorInfoId = value;
                saveMask |= ResidenceSaveMask.Door;
            }
        }

        private ushort doorDecorInfoId;

        public ushort Music
        {
            get => musicId;
            set
            {
                HousingWallpaperInfoEntry entry = GameTableManager.Instance.HousingWallpaperInfo.GetEntry(value);
                if (entry == null)
                    throw new ArgumentOutOfRangeException();

                if ((entry.Flags & 0x100) == 0)
                    throw new ArgumentOutOfRangeException();

                musicId = value;
                saveMask |= ResidenceSaveMask.Music;
            }
        }

        private ushort musicId;

        public ushort Ground
        {
            get => groundWallpaperId;
            set
            {
                HousingWallpaperInfoEntry entry = GameTableManager.Instance.HousingWallpaperInfo.GetEntry(value);
                if (entry == null)
                    throw new ArgumentOutOfRangeException();

                if ((entry.Flags & 0x200) == 0)
                    throw new ArgumentOutOfRangeException();

                groundWallpaperId = value;
                saveMask |= ResidenceSaveMask.Ground;
            }
        }

        private ushort groundWallpaperId;

        public ushort Sky
        {
            get => skyWallpaperId;
            set
            {
                HousingWallpaperInfoEntry entry = GameTableManager.Instance.HousingWallpaperInfo.GetEntry(value);
                if (entry == null)
                    throw new ArgumentOutOfRangeException();

                if ((entry.Flags & 0x40) == 0)
                    throw new ArgumentOutOfRangeException();

                skyWallpaperId = value;
                saveMask |= ResidenceSaveMask.Sky;
            }
        }

        private ushort skyWallpaperId;

        public ResidenceFlags Flags
        {
            get => flags;
            set
            {
                flags = value;
                saveMask |= ResidenceSaveMask.Flags;
            }
        }

        private ResidenceFlags flags;

        public byte ResourceSharing
        {
            get => resourceSharing;
            set
            {
                resourceSharing = value;
                saveMask |= ResidenceSaveMask.ResourceSharing;
            }
        }

        private byte resourceSharing;

        public byte GardenSharing
        {
            get => gardenSharing;
            set
            {
                gardenSharing = value;
                saveMask |= ResidenceSaveMask.GardenSharing;
            }
        }

        private byte gardenSharing;

        private ResidenceSaveMask saveMask;

        public bool IsCommunityResidence => GuildOwnerId.HasValue && !OwnerId.HasValue;

        /// <summary>
        /// <see cref="ResidenceMapInstance"/> this <see cref="Residence"/> resides on.
        /// </summary>
        /// <remarks>
        /// This can either be an individual or shared residencial map.
        /// </remarks>
        public ResidenceMapInstance Map { get; set; }

        /// <summary>
        /// Parent <see cref="Residence"/> for this <see cref="Residence"/>.
        /// </summary>
        /// <remarks>
        /// This will be set if the <see cref="Residence"/> is part of a <see cref="Community"/>.
        /// </remarks>
        public Residence Parent { get; set; }

        /// <summary>
        /// A collection of child <see cref="Residence"/> for this <see cref="Residence"/>.
        /// </summary>
        /// <remarks>
        /// This will contain entries if the <see cref="Residence"/> is the parent for a <see cref="Community"/>.
        /// </remarks>
        private readonly Dictionary<ulong, ResidenceChild> children = new();

        private readonly Dictionary<ulong, Decor> decors = new();
        private readonly List<Plot> plots = new();

        /// <summary>
        /// Create a new <see cref="Residence"/> from an existing database model.
        /// </summary>
        public Residence(ResidenceModel model)
        {
            Id                  = model.Id;
            OwnerId             = model.OwnerId;
            GuildOwnerId        = model.GuildOwnerId;
            propertyInfoId      = (PropertyInfoId)model.PropertyInfoId;
            name                = model.Name;
            privacyLevel        = (ResidencePrivacyLevel)model.PrivacyLevel;
            wallpaperId         = model.WallpaperId;
            roofDecorInfoId     = model.RoofDecorInfoId;
            entrywayDecorInfoId = model.EntrywayDecorInfoId;
            doorDecorInfoId     = model.DoorDecorInfoId;
            groundWallpaperId   = model.GroundWallpaperId;
            musicId             = model.MusicId;
            skyWallpaperId      = model.SkyWallpaperId;
            flags               = (ResidenceFlags)model.Flags;
            resourceSharing     = model.ResourceSharing;
            gardenSharing       = model.GardenSharing;

            // community residences are owned by only a guild
            Type = model.OwnerId.HasValue ? ResidenceType.Residence : ResidenceType.Community;

            foreach (ResidenceDecor decorModel in model.Decor)
            {
                HousingDecorInfoEntry entry = GameTableManager.Instance.HousingDecorInfo.GetEntry(decorModel.DecorInfoId);
                if (entry == null)
                    throw new DatabaseDataException($"Decor {decorModel.Id} has invalid decor entry {decorModel.DecorInfoId}!");

                var decor = new Decor(this, decorModel, entry);
                decors.Add(decor.DecorId, decor);
            }

            foreach (ResidencePlotModel plotModel in model.Plot
                .OrderBy(e => e.Index))
                plots.Add(new Plot(plotModel));

            saveMask = ResidenceSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="Residence"/> from a <see cref="Player"/>.
        /// </summary>
        public Residence(Player player)
        {
            Id             = GlobalResidenceManager.Instance.NextResidenceId;
            Type           = ResidenceType.Residence;
            OwnerId        = player.CharacterId;
            propertyInfoId = PropertyInfoId.Residence;
            name           = $"{player.Name}'s House";
            privacyLevel   = ResidencePrivacyLevel.Public;

            saveMask       = ResidenceSaveMask.Create;

            InitialiseDefaultPlots();

            // TODO: find a better way to do this, this adds the construction yard plug
            plots[0].SetPlug(531);
        }

        /// <summary>
        /// Create a new <see cref="Residence"/> for a <see cref="Community"/>.
        /// </summary>
        /// <remarks>
        /// This creates the parent <see cref="Residence"/> which all children are part of.
        /// </remarks>
        public Residence(Community community)
        {
            Id             = GlobalResidenceManager.Instance.NextResidenceId;
            Type           = ResidenceType.Community;
            GuildOwnerId   = community.Id;
            propertyInfoId = PropertyInfoId.Community;
            name           = community.Name;
            privacyLevel   = ResidencePrivacyLevel.Public;

            saveMask       = ResidenceSaveMask.Create;

            InitialiseDefaultPlots();

            // TODO: find a better way to do this
            plots[0].SetPlug(573);
        }

        private void InitialiseDefaultPlots()
        {
            foreach (HousingPlotInfoEntry entry in GameTableManager.Instance.HousingPlotInfo.Entries
                .Where(e => (PropertyInfoId)e.HousingPropertyInfoId == PropertyInfoId)
                .OrderBy(e => e.HousingPropertyPlotIndex))
                plots.Add(new Plot(Id, entry));
        }

        private void UpdatePlots()
        {
            foreach (HousingPlotInfoEntry entry in GameTableManager.Instance.HousingPlotInfo.Entries
                .Where(e => (PropertyInfoId)e.HousingPropertyInfoId == PropertyInfoId))
                GetPlot((byte)entry.HousingPropertyPlotIndex).PlotInfoEntry = entry;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask != ResidenceSaveMask.None)
            {
                if ((saveMask & ResidenceSaveMask.Create) != 0)
                {
                    // residence doesn't exist in database, all information must be saved
                    context.Add(new ResidenceModel
                    {
                        Id                  = Id,
                        OwnerId             = OwnerId,
                        GuildOwnerId        = GuildOwnerId,
                        PropertyInfoId      = (byte)PropertyInfoId,
                        Name                = Name,
                        PrivacyLevel        = (byte)privacyLevel,
                        WallpaperId         = wallpaperId,
                        RoofDecorInfoId     = roofDecorInfoId,
                        EntrywayDecorInfoId = entrywayDecorInfoId,
                        DoorDecorInfoId     = doorDecorInfoId,
                        GroundWallpaperId   = groundWallpaperId,
                        MusicId             = musicId,
                        SkyWallpaperId      = skyWallpaperId,
                        Flags               = (ushort)flags,
                        ResourceSharing     = resourceSharing,
                        GardenSharing       = gardenSharing
                    });
                }
                else
                {
                    // residence already exists in database, save only data that has been modified
                    var model = new ResidenceModel
                    {
                        Id = Id
                    };

                    // could probably clean this up with reflection, works for the time being
                    EntityEntry<ResidenceModel> entity = context.Attach(model);
                    if ((saveMask & ResidenceSaveMask.Name) != 0)
                    {
                        model.Name = Name;
                        entity.Property(p => p.Name).IsModified = true;
                    }
                    if ((saveMask & ResidenceSaveMask.PrivacyLevel) != 0)
                    {
                        model.PrivacyLevel = (byte)PrivacyLevel;
                        entity.Property(p => p.PrivacyLevel).IsModified = true;
                    }
                    if ((saveMask & ResidenceSaveMask.Wallpaper) != 0)
                    {
                        model.WallpaperId = Wallpaper;
                        entity.Property(p => p.WallpaperId).IsModified = true;
                    }
                    if ((saveMask & ResidenceSaveMask.Roof) != 0)
                    {
                        model.RoofDecorInfoId = Roof;
                        entity.Property(p => p.RoofDecorInfoId).IsModified = true;
                    }
                    if ((saveMask & ResidenceSaveMask.Entryway) != 0)
                    {
                        model.EntrywayDecorInfoId = Entryway;
                        entity.Property(p => p.EntrywayDecorInfoId).IsModified = true;
                    }
                    if ((saveMask & ResidenceSaveMask.Door) != 0)
                    {
                        model.DoorDecorInfoId = Door;
                        entity.Property(p => p.DoorDecorInfoId).IsModified = true;
                    }
                    if ((saveMask & ResidenceSaveMask.Ground) != 0)
                    {
                        model.GroundWallpaperId = Ground;
                        entity.Property(p => p.GroundWallpaperId).IsModified = true;
                    }
                    if ((saveMask & ResidenceSaveMask.Music) != 0)
                    {
                        model.MusicId = Music;
                        entity.Property(p => p.MusicId).IsModified = true;
                    }
                    if ((saveMask & ResidenceSaveMask.Sky) != 0)
                    {
                        model.SkyWallpaperId = Sky;
                        entity.Property(p => p.SkyWallpaperId).IsModified = true;
                    }
                    if ((saveMask & ResidenceSaveMask.Flags) != 0)
                    {
                        model.Flags = (ushort)Flags;
                        entity.Property(p => p.Flags).IsModified = true;
                    }
                    if ((saveMask & ResidenceSaveMask.ResourceSharing) != 0)
                    {
                        model.ResourceSharing = ResourceSharing;
                        entity.Property(p => p.ResourceSharing).IsModified = true;
                    }
                    if ((saveMask & ResidenceSaveMask.GardenSharing) != 0)
                    {
                        model.GardenSharing = GardenSharing;
                        entity.Property(p => p.GardenSharing).IsModified = true;
                    }
                    if ((saveMask & ResidenceSaveMask.GuildOwner) != 0)
                    {
                        model.GuildOwnerId = GuildOwnerId;
                        entity.Property(p => p.GuildOwnerId).IsModified = true;
                    }
                    if ((saveMask & ResidenceSaveMask.PropertyInfo) != 0)
                    {
                        model.PropertyInfoId = (byte)PropertyInfoId;
                        entity.Property(p => p.PropertyInfoId).IsModified = true;
                    }
                }

                saveMask = ResidenceSaveMask.None;
            }

            var decorToRemove = new List<Decor>();
            foreach (Decor decor in decors.Values)
            {
                if (decor.PendingDelete)
                    decorToRemove.Add(decor);

                decor.Save(context);
            }

            foreach (Decor decor in decorToRemove)
                decors.Remove(decor.DecorId);

            foreach (Plot plot in plots)
                plot.Save(context);
        }

        public ServerHousingProperties.Residence Build()
        {
            return new()
            {
                RealmId           = WorldServer.RealmId,
                ResidenceId       = Id,
                NeighbourhoodId   = 0x190000000000000A/*GuildOwnerId.GetValueOrDefault(0ul)*/,
                CharacterIdOwner  = OwnerId,
                GuildIdOwner      = Type == ResidenceType.Community ? GuildOwnerId : 0,
                Type              = Type,
                Name              = Name,
                PropertyInfoId    = PropertyInfoId,
                WallpaperExterior = Wallpaper,
                Entryway          = Entryway,
                Roof              = Roof,
                Door              = Door,
                Ground            = Ground,
                Music             = Music,
                Sky               = Sky,
                Flags             = Flags,
                ResourceSharing   = ResourceSharing,
                GardenSharing     = GardenSharing
            };
        }

        /// <summary>
        /// Return all <see cref="ResidenceChild"/>'s.
        /// </summary>
        /// <remarks>
        /// Only community residences will have child residences.
        /// </remarks>
        public IEnumerable<ResidenceChild> GetChildren()
        {
            return children.Values;
        }

        /// <summary>
        /// Return <see cref="ResidenceChild"/> with supplied property info id.
        /// </summary>
        /// <remarks>
        /// Only community residences will have child residences.
        /// </remarks>
        public ResidenceChild GetChild(PropertyInfoId propertyInfoId)
        {
            return children.Values
                .SingleOrDefault(c => c.Residence.PropertyInfoId == propertyInfoId);
        }


        /// <summary>
        /// Return <see cref="ResidenceChild"/> with supplied character id.
        /// </summary>
        /// <remarks>
        /// Only community residences will have child residences.
        /// </remarks>
        public ResidenceChild GetChild(ulong characterId)
        {
            return children.Values
                .SingleOrDefault(c => c.Residence.OwnerId == characterId);
        }

        /// <summary>
        /// Add child <see cref="Residence"/> to parent <see cref="Residence"/>.
        /// </summary>
        /// <remarks>
        /// Child residences can only be added to a community.
        /// </remarks>
        public void AddChild(Residence residence, bool temporary)
        {
            if (Type != ResidenceType.Community)
                throw new InvalidOperationException("Only community residences can have children!");

            if (children.Any(c => c.Value.Residence.PropertyInfoId == residence.PropertyInfoId))
                throw new ArgumentException();

            children.Add(residence.Id, new ResidenceChild
            {
                Residence   = residence,
                IsTemporary = temporary
            });

            residence.Parent       = this;
            residence.GuildOwnerId = GuildOwnerId;
        }

        /// <summary>
        /// Remove child <see cref="Residence"/> to parent <see cref="Residence"/>.
        /// </summary>
        /// <remarks>
        /// Child residences can only be removed from a community.
        /// </remarks>
        public void RemoveChild(Residence residence)
        {
            if (Type != ResidenceType.Community)
                throw new InvalidOperationException("Only community residences can have children!");

            children.Remove(residence.Id);

            residence.Parent       = null;
            residence.GuildOwnerId = null;
        }

        /// <summary>
        /// Returns true if <see cref="Player"/> can modify the <see cref="Residence"/>.
        /// </summary>
        /// <remarks>
        /// This is valid for both community and individual residences.
        /// </remarks>
        public bool CanModifyResidence(Player player)
        {
            switch (Type)
            {
                case ResidenceType.Community:
                {
                    Community community = player.GuildManager.GetGuild<Community>(GuildType.Community);
                    if (community == null)
                        return false;

                    GuildMember member = community.GetMember(player.CharacterId);
                    if (member == null)
                        return false;

                    return member.Rank.HasPermission(GuildRankPermission.DecorateCommunity);
                }
                case ResidenceType.Residence:
                {
                    // TODO: roommates can also update decor
                    return player.CharacterId == OwnerId;
                }
                default:
                    return false;
            }
        }

        /// <summary>
        /// Return all <see cref="Plot"/>'s for the <see cref="Residence"/>.
        /// </summary>
        public IEnumerable<Plot> GetPlots()
        {
            return plots;
        }

        /// <summary>
        /// Return all <see cref="Decor"/> for the <see cref="Residence"/>.
        /// </summary>
        public IEnumerable<Decor> GetDecor()
        {
            return decors.Values;
        }

        /// <summary>
        /// Return all <see cref="Decor"/> placed in the world for the <see cref="Residence"/>.
        /// </summary>
        public IEnumerable<Decor> GetPlacedDecor()
        {
            foreach (Decor decor in decors.Values)
                if (decor.Type != DecorType.Crate)
                    yield return decor;
        }

        /// <summary>
        /// Return <see cref="Decor"/> with the supplied id.
        /// </summary>
        public Decor GetDecor(ulong decorId)
        {
            decors.TryGetValue(decorId, out Decor decor);
            return decor;
        }

        /// <summary>
        /// Create a new <see cref="Decor"/> from supplied <see cref="HousingDecorInfoEntry"/> for <see cref="Residence"/>.
        /// </summary>
        public Decor DecorCreate(HousingDecorInfoEntry entry)
        {
            var decor = new Decor(this, GlobalResidenceManager.Instance.NextDecorId, entry);
            decors.Add(decor.DecorId, decor);
            return decor;
        }

        /// <summary>
        /// Create a new <see cref="Decor"/> from an existing <see cref="Decor"/>.
        /// </summary>
        /// <remarks>
        /// Copies all data from the source <see cref="Decor"/> with a new id.
        /// </remarks>
        public Decor DecorCopy(Decor decor)
        {
            var newDecor = new Decor(this, decor, GlobalResidenceManager.Instance.NextDecorId);
            decors.Add(newDecor.DecorId, newDecor);
            return newDecor;
        }

        /// <summary>
        /// Remove existing <see cref="Decor"/> from the <see cref="Residence"/>.
        /// </summary>
        /// <remarks>
        /// This does not queue the <see cref="Decor"/> for deletion from the database.
        /// This is intended to be used for <see cref="Decor"/> that has yet to be saved to the database.
        /// </remarks>
        public void DecorRemove(Decor decor)
        {
            decors.Remove(decor.DecorId);
        }

        /// <summary>
        /// Return <see cref="Plot"/> at the supplied index.
        /// </summary>
        public Plot GetPlot(byte plotIndex)
        {
            return plots.FirstOrDefault(i => i.Index == plotIndex);
        }

        /// <summary>
        /// Return <see cref="Plot"/> that matches the supploed Plot Info ID.
        /// </summary>
        public Plot GetPlot(uint plotInfoId)
        {
            return plots.FirstOrDefault(i => i.PlotInfoEntry.Id == plotInfoId);
        }
    }
}
