namespace NexusForever.GameTable.Model
{
    public class SoundZoneKitEntry
    {
        public uint Id { get; set; }
        public uint SoundZoneKitIdParent { get; set; }
        public uint WorldZoneId { get; set; }
        public uint InheritFlags { get; set; }
        public uint PropertyFlags { get; set; }
        public uint SoundMusicSetId { get; set; }
        public uint SoundEventIdIntro { get; set; }
        public float IntroReplayWait { get; set; }
        public uint SoundEventIdMusicMood { get; set; }
        public uint SoundEventIdAmbientDay { get; set; }
        public uint SoundEventIdAmbientNight { get; set; }
        public uint SoundEventIdAmbientUnderwater { get; set; }
        public uint SoundEventIdAmbientStop { get; set; }
        public uint SoundEventIdAmbientPreStopOverride { get; set; }
        public uint SoundEnvironmentId00 { get; set; }
        public uint SoundEnvironmentId01 { get; set; }
        public float EnvironmentDry { get; set; }
        public float EnvironmentWet00 { get; set; }
        public float EnvironmentWet01 { get; set; }
    }
}
