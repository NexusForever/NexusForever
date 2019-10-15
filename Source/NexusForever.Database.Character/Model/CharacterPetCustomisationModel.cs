namespace NexusForever.Database.Character.Model
{
    public class CharacterPetCustomisationModel
    {
        public ulong Id { get; set; }
        public byte Type { get; set; }
        public uint ObjectId { get; set; }
        public string Name { get; set; }
        public ulong FlairIdMask { get; set; }

        public CharacterModel Character { get; set; }
    }
}
