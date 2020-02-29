namespace NexusForever.Database.Character.Model
{
    public class CharacterCustomisationModel
    {
        public ulong Id { get; set; }
        public uint Label { get; set; }
        public uint Value { get; set; }

        public CharacterModel Character { get; set; }
    }
}
