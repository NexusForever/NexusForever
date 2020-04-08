namespace NexusForever.Database.Character.Model
{
    public class CharacterTradeskillMaterialModel
    {
        public ulong Id { get; set; }
        public ushort MaterialId { get; set; }
        public ushort Amount { get; set; }

        public virtual CharacterModel Character { get; set; }
    }
}
