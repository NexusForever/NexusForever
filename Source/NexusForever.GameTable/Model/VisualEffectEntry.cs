namespace NexusForever.GameTable.Model
{
    public class VisualEffectEntry
    {
        public uint Id { get; set; }
        public uint VisualType { get; set; }
        public uint StartDelay { get; set; }
        public uint Duration { get; set; }
        public uint ModelItemSlot { get; set; }
        public uint ModelItemCostumeSide { get; set; }
        public string ModelAssetPath { get; set; }
        public uint ModelAttachmentId { get; set; }
        public uint ModelSequencePriority { get; set; }
        public uint ModelSequenceIdTarget00 { get; set; }
        public uint ModelSequenceIdTarget01 { get; set; }
        public uint ModelSequenceIdTarget02 { get; set; }
        public float ModelScale { get; set; }
        public float ModelRotationX { get; set; }
        public float ModelRotationY { get; set; }
        public float ModelRotationZ { get; set; }
        public float Data00 { get; set; }
        public float Data01 { get; set; }
        public float Data02 { get; set; }
        public float Data03 { get; set; }
        public float Data04 { get; set; }
        public uint Flags { get; set; }
        public uint SoundEventId00 { get; set; }
        public uint SoundEventId01 { get; set; }
        public uint SoundEventId02 { get; set; }
        public uint SoundEventId03 { get; set; }
        public uint SoundEventId04 { get; set; }
        public uint SoundEventId05 { get; set; }
        public uint SoundEventOffset00 { get; set; }
        public uint SoundEventOffset01 { get; set; }
        public uint SoundEventOffset02 { get; set; }
        public uint SoundEventOffset03 { get; set; }
        public uint SoundEventOffset04 { get; set; }
        public uint SoundEventOffset05 { get; set; }
        public uint SoundEventIdStop { get; set; }
        public uint SoundZoneKitId { get; set; }
        public uint PrerequisiteId { get; set; }
        public uint ParticleDiffuseColor { get; set; }
    }
}
