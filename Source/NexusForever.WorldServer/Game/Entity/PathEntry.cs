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
        public uint SoldierLevelRewarded { get; set; }
        public uint SettlerLevelRewarded { get; set; }
        public uint ScientistLevelRewarded { get; set; }
        public uint ExplorerLevelRewarded { get; set; }
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

                //TODO: Handle saving XP and level-up rewards
            }

            saveMask = PathSaveMask.None;
        }
    }
}
