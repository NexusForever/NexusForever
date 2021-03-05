using System;

namespace NexusForever.Database.Character.Model
{
    public interface IAchievementModel
    {
       ulong Id { get; set; }
       ushort AchievementId { get; set; }
       uint Data0 { get; set; }
       uint Data1 { get; set; }
       DateTime? DateCompleted { get; set; }
    }
}
