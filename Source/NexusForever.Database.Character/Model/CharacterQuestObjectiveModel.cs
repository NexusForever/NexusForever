namespace NexusForever.Database.Character.Model
{
    public class CharacterQuestObjectiveModel
    {
        public ulong Id { get; set; }
        public ushort QuestId { get; set; }
        public byte Index { get; set; }
        public uint Progress { get; set; }
        public uint? Timer { get; set; }

        public CharacterQuestModel Quest { get; set; }
    }
}
