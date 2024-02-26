namespace NexusForever.GameTable.Model
{
    public class ClientSideInteractionEntry
    {
        public uint Id { get; set; }
        public uint InteractionType { get; set; }
        public uint Threshold { get; set; }
        public uint Duration { get; set; }
        public uint IncrementValue { get; set; }
        public uint WindowSize { get; set; }
        public uint Decay { get; set; }
        public uint Flags { get; set; }
        public uint TapTime0 { get; set; }
        public uint TapTime1 { get; set; }
        public uint VisualEffectId0 { get; set; }
        public uint VisualEffectId1 { get; set; }
        public uint VisualEffectId2 { get; set; }
        public uint VisualEffectId3 { get; set; }
        public uint VisualEffectIdTarget00 { get; set; }
        public uint VisualEffectIdTarget01 { get; set; }
        public uint VisualEffectIdTarget02 { get; set; }
        public uint VisualEffectIdTarget03 { get; set; }
        public uint VisualEffectIdTarget04 { get; set; }
        public uint VisualEffectIdCaster00 { get; set; }
        public uint VisualEffectIdCaster01 { get; set; }
        public uint VisualEffectIdCaster02 { get; set; }
        public uint VisualEffectIdCaster03 { get; set; }
        public uint VisualEffectIdCaster04 { get; set; }
        public uint LocalizedTextIdContext { get; set; }
    }
}
