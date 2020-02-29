namespace NexusForever.Database.Character.Model
{
    public class CharacterSpellModel
    {
        public ulong Id { get; set; }
        public uint Spell4BaseId { get; set; }
        public byte Tier { get; set; }

        public CharacterModel Character { get; set; }
    }
}
