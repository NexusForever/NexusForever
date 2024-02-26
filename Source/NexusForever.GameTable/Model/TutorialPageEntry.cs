namespace NexusForever.GameTable.Model
{
    public class TutorialPageEntry
    {
        public uint Id { get; set; }
        public uint TutorialId { get; set; }
        public uint Page { get; set; }
        public uint TutorialLayoutId { get; set; }
        public uint LocalizedTextIdTitle { get; set; }
        public uint LocalizedTextIdBody00 { get; set; }
        public uint LocalizedTextIdBody01 { get; set; }
        public uint LocalizedTextIdBody02 { get; set; }
        public string Sprite00 { get; set; }
        public string Sprite01 { get; set; }
        public string Sprite02 { get; set; }
        public uint SoundEventId { get; set; }
    }
}
