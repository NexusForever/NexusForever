using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterAchievement
    {
        public ulong Id { get; set; }
        public ushort AchievementId { get; set; }
        public uint Data0 { get; set; }
        public uint Data1 { get; set; }
        public DateTime? DateCompleted { get; set; }

        public virtual Character IdNavigation { get; set; }
    }
}
