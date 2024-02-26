namespace NexusForever.GameTable.Model
{
    public class ZoneCompletionEntry
    {
        public uint Id { get; set; }
        public uint MapZoneId { get; set; }
        public uint ZoneCompletionFactionEnum { get; set; }
        public uint EpisodeQuestCount { get; set; }
        public uint TaskQuestCount { get; set; }
        public uint ChallengeCount { get; set; }
        public uint DatacubeCount { get; set; }
        public uint TaleCount { get; set; }
        public uint JournalCount { get; set; }
        public uint CharacterTitleIdReward { get; set; }
    }
}
