using System;

namespace NexusForever.Database.Character.Model
{
    public class CharacterAchievementModel
    {
        public ulong Id { get; set; }
        public ushort AchievementId { get; set; }
        public uint Data0 { get; set; }
        public uint Data1 { get; set; }
        public DateTime? DateCompleted { get; set; }

        public CharacterModel Character { get; set; }
    }
}
