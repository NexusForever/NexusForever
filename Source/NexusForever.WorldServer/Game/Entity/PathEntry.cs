using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NLog;

namespace NexusForever.WorldServer.Game.Entity
{
    public class PathEntry : ISaveCharacter
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public ulong CharacterId { get; set; }
        public DateTime PathActivatedTimestamp { get; set; }

        public PathUnlockedMask PathsUnlocked
        {
            get => pathsUnlocked;
            set
            {
                pathsUnlocked |= value;
                saveMask |= PathSaveMask.PathChange;
            }
        }
        private PathUnlockedMask pathsUnlocked;

        public uint SoldierXp
        {
            get => soldierXp;
            set
            {
                if (value < soldierXp)
                    throw new ArgumentException("New Soldier XP Value must be 0 or higher, and not equal to current XP total.");

                soldierXp = value;
                saveMask |= PathSaveMask.XPChange;
            }
        }
        private uint soldierXp;

        public uint SettlerXp
        {
            get => settlerXp;
            set
            {
                if (value < settlerXp)
                    throw new ArgumentException("New Settler XP Value must be 0 or higher, and not equal to current XP total.");

                settlerXp = value;
                saveMask |= PathSaveMask.XPChange;
            }
        }
        private uint settlerXp;

        public uint ScientistXp
        {
            get => scientistXp;
            set
            {
                if (value < scientistXp)
                    throw new ArgumentException("New Scientist XP Value must be 0 or higher, and not equal to current XP total.");

                scientistXp = value;
                saveMask |= PathSaveMask.XPChange;
            }
        }
        private uint scientistXp;

        public uint ExplorerXp
        {
            get => explorerXp;
            set
            {
                if (value < explorerXp)
                    throw new ArgumentException("New Explorer XP Value must be 0 or higher, and not equal to current XP total.");

                explorerXp = value;
                saveMask |= PathSaveMask.XPChange;
            }
        }
        private uint explorerXp;

        public uint SoldierLevelRewarded {
            get => soldierLevelRewarded;
            set
            {
                if (value < soldierLevelRewarded)
                    throw new ArgumentException("New Level Rewarded Value must be higher, and not equal to current XP total.");

                soldierLevelRewarded = value;
                saveMask |= PathSaveMask.LevelChange;
            }
        }
        private uint soldierLevelRewarded;

        public uint SettlerLevelRewarded
        {
            get => settlerLevelRewarded;
            set
            {
                if (value < settlerLevelRewarded)
                    throw new ArgumentException("New Level Rewarded Value must be higher, and not equal to current XP total.");

                settlerLevelRewarded = value;
                saveMask |= PathSaveMask.LevelChange;
            }
        }
        private uint settlerLevelRewarded;

        public uint ScientistLevelRewarded
        {
            get => scientistLevelRewarded;
            set
            {
                if (value < scientistLevelRewarded)
                    throw new ArgumentException("New Level Rewarded Value must be higher, and not equal to current XP total.");

                scientistLevelRewarded = value;
                saveMask |= PathSaveMask.LevelChange;
            }
        }
        private uint scientistLevelRewarded;

        public uint ExplorerLevelRewarded
        {
            get => explorerLevelRewarded;
            set
            {
                if (value <   explorerLevelRewarded)
                    throw new ArgumentException("New Level Rewarded Value must be higher, and not equal to current XP total.");

                explorerLevelRewarded = value;
                saveMask |= PathSaveMask.LevelChange;
            }
        }
        private uint explorerLevelRewarded;

        public Path ActivePath
        {
            get => activePath;
            set
            {
                if (activePath == value && value != 0)
                    throw new ArgumentException("New Active Path must be different than current Active Path");

                activePath = value;
                saveMask |= PathSaveMask.PathChange;
            }
        }

        private Path activePath;

        private PathSaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="PathEntry"/> for a <see cref="Player"/> from <see cref="CharacterPath"/>
        /// </summary>
        public PathEntry(CharacterPath model)
        {
            CharacterId = model.Id;
            ActivePath = (Path)model.ActivePath;
            PathsUnlocked = (PathUnlockedMask)model.PathsUnlocked;
            SoldierXp = model.SoldierXp;
            SettlerXp = model.SettlerXp;
            ScientistXp = model.ScientistXp;
            ExplorerXp = model.ExplorerXp;
            SoldierLevelRewarded = model.SoldierLevelRewarded;
            SettlerLevelRewarded = model.SettlerLevelRewarded;
            ScientistLevelRewarded = model.ScientistLevelRewarded;
            ExplorerLevelRewarded = model.ExplorerLevelRewarded;

            saveMask = PathSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="PathEntry"/>
        /// </summary>
        public PathEntry(ulong owner, Path activePath, PathUnlockedMask pathsUnlocked)
        {
            CharacterId = owner;
            ActivePath = activePath;
            PathsUnlocked = pathsUnlocked;

            saveMask = PathSaveMask.Create;
        }

        /// <summary>
        /// Save the <see cref="CharacterPath"/> with it's current state
        /// </summary>
        /// <param name="context">The character context to save against</param>
        public void Save(CharacterContext context)
        {
            if (saveMask == PathSaveMask.None)
                return;

            if ((saveMask & PathSaveMask.Create) != 0)
            {
                // Currency doesn't exist in database, all infomation must be saved
                context.Add(new CharacterPath
                {
                    Id = CharacterId,
                    ActivePath = (byte)ActivePath,
                    PathsUnlocked = (ushort)PathsUnlocked
                });
            }
            else
            {
                // Currency already exists in database, save only data that has been modified
                var model = new CharacterPath
                {
                    Id = CharacterId
                };

                EntityEntry<CharacterPath> entity = context.Attach(model);
                if ((saveMask & PathSaveMask.PathChange) != 0)
                {
                    model.ActivePath = (byte)ActivePath;
                    entity.Property(p => p.ActivePath).IsModified = true;

                    model.PathsUnlocked = (ushort)PathsUnlocked;
                    entity.Property(p => p.PathsUnlocked).IsModified = true;
                }

                if ((saveMask & PathSaveMask.XPChange) != 0)
                {
                    model.SoldierXp = SoldierXp;
                    entity.Property(p => p.SoldierXp).IsModified = true;

                    model.SettlerXp = SettlerXp;
                    entity.Property(p => p.SettlerXp).IsModified = true;

                    model.ScientistXp = ScientistXp;
                    entity.Property(p => p.ScientistXp).IsModified = true;

                    model.ExplorerXp = ExplorerXp;
                    entity.Property(p => p.ExplorerXp).IsModified = true;
                }

                if ((saveMask & PathSaveMask.LevelChange) != 0)
                {
                    model.SoldierLevelRewarded = SoldierLevelRewarded;
                    entity.Property(p => p.SoldierLevelRewarded).IsModified = true;

                    model.SettlerLevelRewarded = SettlerLevelRewarded;
                    entity.Property(p => p.SettlerLevelRewarded).IsModified = true;

                    model.ScientistLevelRewarded = ScientistLevelRewarded;
                    entity.Property(p => p.ScientistLevelRewarded).IsModified = true;

                    model.ExplorerLevelRewarded = ExplorerLevelRewarded;
                    entity.Property(p => p.ExplorerLevelRewarded).IsModified = true;
                }
            }

            saveMask = PathSaveMask.None;
        }
    }
}
