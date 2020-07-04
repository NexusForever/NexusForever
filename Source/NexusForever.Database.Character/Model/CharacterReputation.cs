namespace NexusForever.Database.Character.Model
{
    public class CharacterReputation
    {
        public ulong Id { get; set; }
        public uint FactionId { get; set; }
        public float Amount { get; set; }

        public CharacterModel Character { get; set; }
    }
}
