namespace NexusForever.GameTable.Model
{
    public class EpisodeQuestEntry
    {
        public uint Id { get; set; }
        public uint EpisodeId { get; set; }
        public uint QuestId { get; set; }
        public uint OrderIdx { get; set; }
        public uint Flags { get; set; }
    }
}
