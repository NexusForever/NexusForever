using System;
using System.Linq;
using NexusForever.Database.Character;
using NexusForever.Shared.Network.Message;
using AchievementNetworkModel = NexusForever.WorldServer.Network.Message.Model.Shared.Achievement;

namespace NexusForever.WorldServer.Game.Achievement
{
    public abstract class Achievement : ISaveCharacter, IBuildable<AchievementNetworkModel>
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

        /// <summary>
        /// Create a new <see cref="Achievement"/> from <see cref="AchievementInfo"/> and supplied data.
        /// </summary>
        protected Achievement(AchievementInfo info, uint data0, uint data1, DateTime? dateCompleted, bool save)
        {
            Info               = info;
            this.data0         = data0;
            this.data1         = data1;
            this.dateCompleted = dateCompleted;

            if (save)
                saveMask |= SaveMask.Create;
        }

        public abstract void Save(CharacterContext context);

        /// <summary>
        /// Build a network model from <see cref="Achievement"/>.
        /// </summary>
        public AchievementNetworkModel Build()
        {
            return new AchievementNetworkModel
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
