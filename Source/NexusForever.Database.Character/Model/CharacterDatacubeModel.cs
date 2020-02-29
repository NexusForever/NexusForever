namespace NexusForever.Database.Character.Model
{
    public class CharacterDatacubeModel
    {
        public ulong Id { get; set; }
        public byte Type { get; set; }
        public ushort Datacube { get; set; }
        public uint Progress { get; set; }

        public CharacterModel Character { get; set; }
    }
}
