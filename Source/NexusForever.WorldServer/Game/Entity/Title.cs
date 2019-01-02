using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Shared;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Game.Entity
{
    public class Title : IUpdate, ISaveCharacter
    {
        public ulong CharacterId { get; }
        public CharacterTitleEntry Entry { get; }

        /// <summary>
        /// <see cref="Title"/> lifetime in milliseconds.
        /// </summary>
        public double? TimeRemaining
        {
            get => timeRemaining;
            set
            {
                timeRemaining = value;
                saveMask |= TitleSaveMask.TimeRemaining;
            }
        }

        private double? timeRemaining;

        // TODO: move to more generic place
        public bool SavedToDatabase { get; private set; }

        private TitleSaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="Title"/> from an existing database model.
        /// </summary>
        public Title(CharacterTitle model)
        {
            CharacterId = model.Id;
            Entry       = GameTableManager.CharacterTitle.GetEntry(model.Title);

            if (Entry.LifeTimeSeconds != 0u)
                timeRemaining = model.TimeRemaining;

            SavedToDatabase = true;
        }

        /// <summary>
        /// Create a new <see cref="Title"/> from a <see cref="CharacterTitleEntry"/> template.
        /// </summary>
        public Title(ulong owner, CharacterTitleEntry entry)
        {
            CharacterId = owner;
            Entry       = entry;

            if (Entry.LifeTimeSeconds != 0u)
                timeRemaining = Entry.LifeTimeSeconds;

            SavedToDatabase = false;
        }

        public void Update(double lastTick)
        {
            if (TimeRemaining != null)
                TimeRemaining -= lastTick;
        }

        public void Save(CharacterContext context)
        {
            if (!SavedToDatabase)
            {
                // title doesn't exist in database, all infomation must be saved
                context.Add(new CharacterTitle
                {
                    Id            = CharacterId,
                    Title         = (ushort)Entry.Id,
                    TimeRemaining = (uint)(timeRemaining ?? 0d)
                });

                SavedToDatabase = true;
            }
            else
            {
                if (saveMask == TitleSaveMask.None)
                    return;

                // title already exists in database, save only data that has been modified
                var model = new CharacterTitle
                {
                    Id    = CharacterId,
                    Title = (ushort)Entry.Id
                };

                EntityEntry<CharacterTitle> entity = context.Attach(model);
                if ((saveMask & TitleSaveMask.TimeRemaining) != 0)
                {
                    // timeRemaining should never be null here, explicit check?
                    model.TimeRemaining = (uint)(timeRemaining ?? 0d);
                    entity.Property(p => p.TimeRemaining).IsModified = true;
                }

                saveMask = TitleSaveMask.None;
            }
        }

        public void Delete(CharacterContext context)
        {
            var model = new CharacterTitle
            {
                Id    = CharacterId,
                Title = (ushort)Entry.Id
            };

            context.Entry(model).State = EntityState.Deleted;
        }
    }
}
