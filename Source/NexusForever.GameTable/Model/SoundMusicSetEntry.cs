namespace NexusForever.GameTable.Model
{
    public class SoundMusicSetEntry
    {
        public uint Id { get; set; }
        public uint SoundEventIdStart { get; set; }
        public uint SoundEventIdStop { get; set; }
        public float RestartDelayMin { get; set; }
        public float RestartDelayMax { get; set; }
        public uint Flags { get; set; }
    }
}
