using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
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

        public bool Revoked
        {
            get => revoked;
            set
            {
                revoked = value;
                saveMask |= TitleSaveMask.Revoked;
            }
        }

        private bool revoked;

        private TitleSaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="Title"/> from an existing database model.
        /// </summary>
        public Title(CharacterTitleModel model)
        {
            CharacterId = model.Id;
            Entry       = GameTableManager.Instance.CharacterTitle.GetEntry(model.Title);
            revoked     = Convert.ToBoolean(model.Revoked);

            if (Entry.LifeTimeSeconds != 0u)
                timeRemaining = model.TimeRemaining;

            saveMask = TitleSaveMask.None;
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

            saveMask = TitleSaveMask.Create;
        }

        public void Update(double lastTick)
        {
            if (TimeRemaining != null)
                TimeRemaining -= lastTick;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask == TitleSaveMask.None)
                return;

            if ((saveMask & TitleSaveMask.Create) != 0)
            {
                // title doesn't exist in database, all infomation must be saved
                context.Add(new CharacterTitleModel
                {
                    Id            = CharacterId,
                    Title         = (ushort)Entry.Id,
                    TimeRemaining = (uint)(timeRemaining ?? 0d)
                });
            }
            else
            {
                // title already exists in database, save only data that has been modified
                var model = new CharacterTitleModel
                {
                    Id    = CharacterId,
                    Title = (ushort)Entry.Id
                };

                EntityEntry<CharacterTitleModel> entity = context.Attach(model);
                if ((saveMask & TitleSaveMask.TimeRemaining) != 0)
                {
                    // timeRemaining should never be null here, explicit check?
                    model.TimeRemaining = (uint)(timeRemaining ?? 0d);
                    entity.Property(p => p.TimeRemaining).IsModified = true;
                }

                if ((saveMask & TitleSaveMask.Revoked) != 0)
                {
                    model.Revoked = Convert.ToByte(Revoked);
                    entity.Property(p => p.Revoked).IsModified = true;
                }
            }

            saveMask = TitleSaveMask.None;
        }
    }
}
