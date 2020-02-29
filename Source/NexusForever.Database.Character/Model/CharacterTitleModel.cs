namespace NexusForever.Database.Character.Model
{
    public class CharacterTitleModel
    {
        public ulong Id { get; set; }
        public ushort Title { get; set; }
        public uint TimeRemaining { get; set; }
        public byte Revoked { get; set; }

        public CharacterModel Character { get; set; }
    }
}
