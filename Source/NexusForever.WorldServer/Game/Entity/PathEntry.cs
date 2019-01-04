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

        public Path Path { get; set;  }
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
        /// Create a new <see cref="PathEntry"/> for a <see cref="Player"/> from <see cref="CharacterPath"/>
        /// </summary>
        public PathEntry(CharacterPath model)
        {
            CharacterId = model.Id;
            Path = (Path)Enum.Parse(typeof(Path), model.PathName);
            Unlocked = model.Unlocked;
            TotalXp = model.TotalXp;
            LevelRewarded = model.LevelRewarded;
            
            saveMask = PathSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="PathEntry"/>
        /// </summary>
        public PathEntry(ulong owner, Path path, bool isUnlocked)
        {
            CharacterId = owner;
            Path = path;
            Unlocked = isUnlocked;

            saveMask = PathSaveMask.Create;
        }

        /// <summary>
        /// Save the <see cref="CharacterPath"/> with it's current state
        /// </summary>
        /// <param name="context">The character context to save against</param>
        public void Save(CharacterContext context)
        {
            log.Debug($"PathEntry.Save called");

            if (saveMask == PathSaveMask.None)
                return;

            if ((saveMask & PathSaveMask.Create) != 0)
            {
                log.Debug($"PathSaveMask == Create");
                // Path doesn't exist in database, all infomation must be saved
                context.Add(new CharacterPath
                {
                    Id = CharacterId,
                    PathName = Path.ToString(),
                    Unlocked = Unlocked,
                    TotalXp = TotalXp,
                    LevelRewarded = LevelRewarded
                });
            }
            else
            {
                // Path already exists in database, save only data that has been modified
                var model = new CharacterPath
                {
                    Id = CharacterId,
                    PathName = Path.ToString()
                };

                EntityEntry<CharacterPath> entity = context.Attach(model);
                if ((saveMask & PathSaveMask.Unlocked) != 0)
                {
                    log.Debug($"PathSaveMask == Unlocked");
                    model.Unlocked = Unlocked;
                    entity.Property(p => p.Unlocked).IsModified = true;
                }

                if ((saveMask & PathSaveMask.XPChange) != 0)
                {
                    log.Debug($"PathSaveMask == XPChange");
                    model.TotalXp = TotalXp;
                    entity.Property(p => p.TotalXp).IsModified = true;
                }

                if ((saveMask & PathSaveMask.LevelChange) != 0)
                {
                    log.Debug($"PathSaveMask == LevelChange");
                    model.LevelRewarded = LevelRewarded;
                    entity.Property(p => p.LevelRewarded).IsModified = true;
                }
            }

            saveMask = PathSaveMask.None;
        }
    }
}
