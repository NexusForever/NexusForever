namespace NexusForever.GameTable.Model
{
    public class FullScreenEffectEntry
    {
        public uint Id { get; set; }
        public string Description { get; set; }
        public string TexturePath { get; set; }
        public string ModelPath { get; set; }
        public uint Priority { get; set; }
        public uint FullScreenEffectTypeEnum { get; set; }
        public float AlphaMinStart { get; set; }
        public float AlphaMinEnd { get; set; }
        public float AlphaStart { get; set; }
        public float AlphaEnd { get; set; }
        public float HzStart { get; set; }
        public float HzEnd { get; set; }
        public float SaturationStart { get; set; }
        public float SaturationEnd { get; set; }
    }
}
