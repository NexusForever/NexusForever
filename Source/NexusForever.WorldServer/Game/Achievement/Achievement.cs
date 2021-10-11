using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared.Network.Message;
using AchievementNetworkModel = NexusForever.WorldServer.Network.Message.Model.Shared.Achievement;

namespace NexusForever.WorldServer.Game.Achievement
{
    public class Achievement<T> : ISaveCharacter, IBuildable<AchievementNetworkModel>
        where T : class, IAchievementModel, new()
    {
        [Flags]
        protected enum SaveMask
        {
            None          = 0x00,
            Create        = 0x01,
            Data0         = 0x02,
            Data1         = 0x04,
            TimeCompleted = 0x08
        }

        public AchievementInfo Info { get; }
        public ushort Id => Info.Id;

        public uint Data0
        {
            get => data0;
            set
            {
                saveMask |= SaveMask.Data0;
                data0 = value;
            }
        }

        private uint data0;

        public uint Data1
        {
            get => data1;
            set
            {
                saveMask |= SaveMask.Data1;
                data1 = value;
            }
        }

        private uint data1;

        public DateTime? DateCompleted
        {
            get => dateCompleted;
            set
            {
                saveMask |= SaveMask.TimeCompleted;
                dateCompleted = value;
            }
        }

        private DateTime? dateCompleted;

        protected SaveMask saveMask;

        // this can either be a characterId or guildId depending on the achievement type
        private readonly ulong ownerId;

        /// <summary>
        /// Create a new <see cref="Achievement{T}"/> from an existing database model.
        /// </summary>
        public Achievement(AchievementInfo info, IAchievementModel model)
        {
            ownerId       = model.Id;
            Info          = info;
            data0         = model.Data0;
            data1         = model.Data1;
            DateCompleted = model.DateCompleted;
        }

        /// <summary>
        /// Create a new <see cref="Achievement{T}"/> from <see cref="AchievementInfo"/> and supplied data.
        /// </summary>
        public Achievement(ulong ownerId, AchievementInfo info)
        {
            this.ownerId = ownerId;
            Info         = info;

            saveMask |= SaveMask.Create;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask == SaveMask.None)
                return;

            if ((saveMask & SaveMask.Create) != 0)
            {
                context.Add(new T
                {
                    Id            = ownerId,
                    AchievementId = Id,
                    Data0         = Data0,
                    Data1         = Data1,
                    DateCompleted = DateCompleted
                });
            }
            else
            {
                var model = new T
                {
                    Id            = ownerId,
                    AchievementId = Id
                };

                EntityEntry<T> entity = context.Attach(model);
                if ((saveMask & SaveMask.Data0) != 0)
                {
                    model.Data0 = Data0;
                    entity.Property(p => p.Data0).IsModified = true;
                }
                if ((saveMask & SaveMask.Data1) != 0)
                {
                    model.Data1 = Data1;
                    entity.Property(p => p.Data1).IsModified = true;
                }
                if ((saveMask & SaveMask.TimeCompleted) != 0)
                {
                    model.DateCompleted = DateCompleted;
                    entity.Property(p => p.DateCompleted).IsModified = true;
                }
            }

            saveMask = SaveMask.None;
        }

        /// <summary>
        /// Build a network model from <see cref="Achievement"/>.
        /// </summary>
        public AchievementNetworkModel Build()
        {
            return new()
            {
                AchievementId = Id,
                Data0         = Data0,
                Data1         = Data1,
                DateCompleted = (ulong)(DateCompleted?.ToFileTimeUtc() ?? 0L)
            };
        }

        /// <summary>
        /// Returns if <see cref="Achievement"/> has been completed.
        /// </summary>
        public bool IsComplete()
        {
            if (DateCompleted != null)
                return true;

            if (Info.ChecklistEntries.Count == 0)
                return Data0 == Info.Entry.Value;

            return Info.ChecklistEntries.All(entry => (Data0 & (1u << (int)entry.Bit)) != 0);
        }
    }
}
