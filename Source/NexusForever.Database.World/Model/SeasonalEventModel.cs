using System.Collections.Generic;

namespace NexusForever.Database.World.Model
{
    public class SeasonalEventModel
    {
        public uint Id { get; set; }
        public byte StartMonth { get; set; }
        public byte StartDay { get; set; }
        public byte StartHourUtc { get; set; }
        public ushort DurationHours { get; set; }
        public ICollection<SeasonalEventQuestModel> Quests { get; set; } = new List<SeasonalEventQuestModel>();
        public ICollection<SeasonalEventEntityModel> Entities { get; set; } = new List<SeasonalEventEntityModel>();
    }

    public class SeasonalEventQuestModel
    {
        public uint EventId { get; set; }
        public uint QuestId { get; set; }
    }

    public class SeasonalEventEntityModel
    {
        public uint EventId { get; set; }
        public uint EntityId { get; set; }
    }
}
