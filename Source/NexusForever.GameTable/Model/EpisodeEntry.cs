namespace NexusForever.GameTable.Model
{
    public class EpisodeEntry
    {
        public uint Id { get; set; }
        public uint LocalizedTextIdName { get; set; }
        public uint LocalizedTextIdBriefing { get; set; }
        public uint LocalizedTextIdEndSummary { get; set; }
        public uint Flags { get; set; }
        public uint WorldZoneId { get; set; }
        public uint PercentToDisplay { get; set; }
        public uint QuestHubIdExile { get; set; }
        public uint QuestHubIdDominion { get; set; }
    }
}
