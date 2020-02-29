namespace NexusForever.Database.Character.Model
{
    public class CharacterPetFlairModel
    {
        public ulong Id { get; set; }
        public uint PetFlairId { get; set; }

        public CharacterModel Character { get; set; }
    }
}
