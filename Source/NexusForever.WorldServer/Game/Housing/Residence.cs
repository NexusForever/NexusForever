﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Housing.Static;
using ResidenceModel = NexusForever.WorldServer.Database.Character.Model.Residence;

namespace NexusForever.WorldServer.Game.Housing
{
    public class Residence : ISaveCharacter
    {
        public ulong Id { get; }
        public ulong OwnerId { get; }
        public string OwnerName { get; }
        public byte PropertyInfoId { get; }

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
                if (GameTableManager.HousingWallpaperInfo.GetEntry(value) == null)
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
                if (GameTableManager.HousingDecorInfo.GetEntry(value) == null)
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
                if (GameTableManager.HousingDecorInfo.GetEntry(value) == null)
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
                if (GameTableManager.HousingDecorInfo.GetEntry(value) == null)
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
                HousingWallpaperInfoEntry entry = GameTableManager.HousingWallpaperInfo.GetEntry(value);
                if (entry == null)
                    throw new ArgumentOutOfRangeException();

                if ((entry.Flags & 0x180) == 0)
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
                HousingWallpaperInfoEntry entry = GameTableManager.HousingWallpaperInfo.GetEntry(value);
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
                HousingWallpaperInfoEntry entry = GameTableManager.HousingWallpaperInfo.GetEntry(value);
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

        private readonly Dictionary<ulong, Decor> decors = new Dictionary<ulong, Decor>();
        private readonly HashSet<Decor> deletedDecors = new HashSet<Decor>();
        private readonly Plot[] plots = new Plot[7];

        /// <summary>
        /// Create a new <see cref="Residence"/> from an existing database model.
        /// </summary>
        public Residence(ResidenceModel model)
        {
            Id                  = model.Id;
            OwnerId             = model.OwnerId;
            OwnerName           = model.Owner.Name;
            PropertyInfoId      = model.PropertyInfoId;
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

            foreach (ResidenceDecor decorModel in model.ResidenceDecor)
            {
                var decor = new Decor(decorModel);
                decors.Add(decor.DecorId, decor);
            }

            foreach (ResidencePlot plotModel in model.ResidencePlot)
            {
                var plot = new Plot(plotModel);
                plots[plot.Index] = plot;
            }

            saveMask = ResidenceSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="Residence"/> from a <see cref="Player"/>.
        /// </summary>
        public Residence(Player player)
        {
            Id             = ResidenceManager.NextResidenceId;
            OwnerId        = player.CharacterId;
            OwnerName      = player.Name;
            PropertyInfoId = 35; // TODO: 35 is default for single residence, this will need to change for communities
            name           = $"{player.Name}'s House";
            privacyLevel   = ResidencePrivacyLevel.Public;

            IEnumerable<HousingPlotInfoEntry> plotEntries = GameTableManager.HousingPlotInfo.Entries.Where(e => e.HousingPropertyInfoId == PropertyInfoId);
            foreach (HousingPlotInfoEntry entry in plotEntries)
            {
                var plot = new Plot(Id, entry);
                plots[plot.Index] = plot;
            }

            // TODO: find a better way to do this, this adds the construction yard plug
            plots[0].SetPlug(531);

            saveMask = ResidenceSaveMask.Create;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask != ResidenceSaveMask.None)
            {
                if ((saveMask & ResidenceSaveMask.Create) != 0)
                {
                    // residence doesn't exist in database, all infomation must be saved
                    context.Add(new ResidenceModel
                    {
                        Id                  = Id,
                        OwnerId             = OwnerId,
                        PropertyInfoId      = PropertyInfoId,
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
                        Id             = Id,
                        OwnerId        = OwnerId,
                        PropertyInfoId = PropertyInfoId
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
                        model.Flags = Sky;
                        entity.Property(p => p.Flags).IsModified = true;
                    }
                }

                saveMask = ResidenceSaveMask.None;
            }

            foreach (Decor decor in decors.Values)
                decor.Save(context);

            foreach (Plot plot in plots)
                plot.Save(context);
        }

        /// <summary>
        /// Returns true if the supplied character id can modify the <see cref="Residence"/>.
        /// </summary>
        public bool CanModifyResidence(ulong characterId)
        {
            // TODO: roommates can also update decor
            return characterId == OwnerId;
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

        public Decor DecorCreate(HousingDecorInfoEntry entry)
        {
            var decor = new Decor(Id, ResidenceManager.NextDecorId, entry);
            decors.Add(decor.DecorId, decor);
            return decor;
        }

        public void DecorDelete(Decor decor)
        {
            decor.EnqueueDelete();

            decors.Remove(decor.DecorId);
            deletedDecors.Add(decor);
        }
    }
}
