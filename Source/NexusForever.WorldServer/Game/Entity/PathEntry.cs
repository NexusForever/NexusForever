using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
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
        public PathUnlocked PathsUnlocked { get; set; }
        public uint SoldierXp { get; set; }
        public uint SettlerXp { get; set; }
        public uint ScientistXp { get; set; }
        public uint ExplorerXp { get; set; }
        public uint SoldierLevelRewarded { get; set; }
        public uint SettlerLevelRewarded { get; set; }
        public uint ScientistLevelRewarded { get; set; }
        public uint ExplorerLevelRewarded { get; set; }

        public Path ActivePath
        {
            get => activePath;
            set
            {
                if (activePath == value && value != 0)
                    throw new ArgumentException("New Active Path must be different than current Active Path");
                activePath = value;
                saveMask |= PathSaveMask.Change;
            }
        }

        private Path activePath;

        private PathSaveMask saveMask;

        public PathEntry()
        {

        }

        /// <summary>
        /// Create a new <see cref="PathEntry"/> for a <see cref="Player"/> from <see cref="CharacterPath"/>
        /// </summary>
        public PathEntry(CharacterPath model)
        {
            CharacterId = model.Id;
            ActivePath = (Path)model.ActivePath;
            PathsUnlocked = (PathUnlocked)model.PathsUnlocked;
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
        public PathEntry(ulong owner, Path activePath, PathUnlocked pathsUnlocked)
        {
            CharacterId = owner;
            ActivePath = activePath;
            PathsUnlocked = pathsUnlocked;

            log.Info("Received new PathEntry. Setting save mask.");

            saveMask = PathSaveMask.Create;
        }

        public void Save(CharacterContext context)
        {
            log.Info("PathEntry.Save Called");

            if (saveMask == PathSaveMask.None)
            {
                log.Info("PathSaveMask == None");
                return;
            }

            if ((saveMask & PathSaveMask.Create) != 0)
            {
                log.Info("PathSaveMask == Create");
                // Currency doesn't exist in database, all infomation must be saved
                context.Add(new CharacterPath
                {
                    Id = CharacterId,
                    ActivePath = (byte)ActivePath,
                    PathsUnlocked = (ushort)PathsUnlocked
                });
            }
            //else
            //{
            //    // Currency already exists in database, save only data that has been modified
            //    var model = new CharacterCurrency
            //    {
            //        Id = CharacterId,
            //        CurrencyId = (byte)Entry.Id,
            //    };

            //    // could probably clean this up with reflection, works for the time being
            //    EntityEntry<CharacterCurrency> entity = context.Attach(model);
            //    if ((saveMask & CurrencySaveMask.Amount) != 0)
            //    {
            //        model.Amount = Amount;
            //        entity.Property(p => p.Amount).IsModified = true;
            //    }
            //}

            log.Info("Clearing PathSaveMask");
            saveMask = PathSaveMask.None;
        }
    }
}
