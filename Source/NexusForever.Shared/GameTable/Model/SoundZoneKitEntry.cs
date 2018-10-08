namespace NexusForever.Shared.GameTable.Model
{
    public class SoundZoneKitEntry
    {
        public uint Id;
        public uint SoundZoneKitIdParent;
        public uint WorldZoneId;
        public uint InheritFlags;
        public uint PropertyFlags;
        public uint SoundMusicSetId;
        public uint SoundEventIdIntro;
        public float IntroReplayWait;
        public uint SoundEventIdMusicMood;
        public uint SoundEventIdAmbientDay;
        public uint SoundEventIdAmbientNight;
        public uint SoundEventIdAmbientUnderwater;
        public uint SoundEventIdAmbientStop;
        public uint SoundEventIdAmbientPreStopOverride;
        public uint SoundEnvironmentId00;
        public uint SoundEnvironmentId01;
        public float EnvironmentDry;
        public float EnvironmentWet00;
        public float EnvironmentWet01;
    }
}
