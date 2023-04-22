﻿using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using Path = NexusForever.Game.Static.Entity.Path;

namespace NexusForever.Game.Entity
{
    public class PathEntry : IPathEntry
    {
        /// <summary>
        /// Determines which fields need saving for <see cref="IPathEntry"/> when being saved to the database.
        /// </summary>
        [Flags]
        public enum PathSaveMask
        {
            None        = 0x0000,
            Create      = 0x0001,
            Unlocked    = 0x0002,
            XPChange    = 0x0004,
            LevelChange = 0x0008
        }

        public Path Path { get; set; }
        public ulong CharacterId { get; set; }

        public bool Unlocked
        {
            get => unlocked;
            set
            {
                if (value != unlocked)
                {
                    unlocked = value;
                    saveMask |= PathSaveMask.Unlocked;
                }
            }
        }
        private bool unlocked;

        public uint TotalXp
        {
            get => totalXp;
            set
            {
                if (value < totalXp)
                    throw new ArgumentException("New Level Rewarded Value must be higher, and not equal to current XP total.");

                totalXp = value;
                saveMask |= PathSaveMask.XPChange;
            }
        }
        private uint totalXp;

        public byte LevelRewarded
        {
            get => levelRewarded;
            set
            {
                if (value < levelRewarded)
                    throw new ArgumentException("New Level Rewarded Value must be higher, and not equal to current XP total.");

                levelRewarded = value;
                saveMask |= PathSaveMask.LevelChange;
            }
        }
        private byte levelRewarded;

        private PathSaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="IPathEntry"/> for a <see cref="IPlayer"/> from <see cref="CharacterPathModel"/>
        /// </summary>
        public PathEntry(CharacterPathModel model)
        {
            CharacterId   = model.Id;
            Path          = (Path)model.Path;
            unlocked      = Convert.ToBoolean(model.Unlocked);
            totalXp       = model.TotalXp;
            levelRewarded = model.LevelRewarded;

            saveMask      = PathSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="IPathEntry"/>
        /// </summary>
        public PathEntry(ulong owner, Path path, bool isUnlocked)
        {
            CharacterId = owner;
            Path        = path;
            unlocked    = isUnlocked;

            saveMask    = PathSaveMask.Create;
        }

        /// <summary>
        /// Save the <see cref="CharacterPathModel"/> with it's current state
        /// </summary>
        /// <param name="context">The character context to save against</param>
        public void Save(CharacterContext context)
        {
            if (saveMask == PathSaveMask.None)
                return;

            if ((saveMask & PathSaveMask.Create) != 0)
            {
                // Path doesn't exist in database, all infomation must be saved
                context.Add(new CharacterPathModel
                {
                    Id            = CharacterId,
                    Path          = (byte)Path,
                    Unlocked      = Convert.ToByte(Unlocked),
                    TotalXp       = TotalXp,
                    LevelRewarded = LevelRewarded
                });
            }
            else
            {
                // Path already exists in database, save only data that has been modified
                var model = new CharacterPathModel
                {
                    Id   = CharacterId,
                    Path = (byte)Path
                };

                EntityEntry<CharacterPathModel> entity = context.Attach(model);
                if ((saveMask & PathSaveMask.Unlocked) != 0)
                {
                    model.Unlocked = Convert.ToByte(Unlocked);
                    entity.Property(p => p.Unlocked).IsModified = true;
                }

                if ((saveMask & PathSaveMask.XPChange) != 0)
                {
                    model.TotalXp = TotalXp;
                    entity.Property(p => p.TotalXp).IsModified = true;
                }

                if ((saveMask & PathSaveMask.LevelChange) != 0)
                {
                    model.LevelRewarded = LevelRewarded;
                    entity.Property(p => p.LevelRewarded).IsModified = true;
                }
            }

            saveMask = PathSaveMask.None;
        }
    }
}
