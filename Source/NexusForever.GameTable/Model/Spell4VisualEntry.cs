namespace NexusForever.GameTable.Model
{
    public class Spell4VisualEntry
    {
        public uint Id { get; set; }
        public uint TargetTypeFlags { get; set; }
        public uint StageType { get; set; }
        public uint StageFlags { get; set; }
        public uint VisualEffectId { get; set; }
        public uint VisualEffectIdSound { get; set; }
        public uint ModelEventIdDelay { get; set; }
        public uint SoundEventType00 { get; set; }
        public uint SoundEventType01 { get; set; }
        public uint SoundEventType02 { get; set; }
        public uint SoundEventType03 { get; set; }
        public uint SoundEventType04 { get; set; }
        public uint SoundEventType05 { get; set; }
        public uint SoundImpactDescriptionIdTarget00 { get; set; }
        public uint SoundImpactDescriptionIdTarget01 { get; set; }
        public uint SoundImpactDescriptionIdTarget02 { get; set; }
        public uint SoundImpactDescriptionIdTarget03 { get; set; }
        public uint SoundImpactDescriptionIdTarget04 { get; set; }
        public uint SoundImpactDescriptionIdTarget05 { get; set; }
        public uint SoundImpactDescriptionIdOrigin00 { get; set; }
        public uint SoundImpactDescriptionIdOrigin01 { get; set; }
        public uint SoundImpactDescriptionIdOrigin02 { get; set; }
        public uint SoundImpactDescriptionIdOrigin03 { get; set; }
        public uint SoundImpactDescriptionIdOrigin04 { get; set; }
        public uint SoundImpactDescriptionIdOrigin05 { get; set; }
        public uint ModelAttachmentIdCaster { get; set; }
        public uint PhaseFlags { get; set; }
        public float ModelOffsetX { get; set; }
        public float ModelOffsetY { get; set; }
        public float ModelOffsetZ { get; set; }
        public float ModelScale { get; set; }
        public uint PreDelayTimeMs { get; set; }
        public uint TelegraphDamageIdAttach { get; set; }
        public uint PrerequisiteId { get; set; }
    }
}
