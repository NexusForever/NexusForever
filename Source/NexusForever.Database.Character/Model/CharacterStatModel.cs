namespace NexusForever.Database.Character.Model
{
    public class CharacterStatModel
    {
        public ulong Id { get; set; }
        public byte Stat { get; set; }
        public float Value { get; set; }

        public CharacterModel Character { get; set; }
    }
}
