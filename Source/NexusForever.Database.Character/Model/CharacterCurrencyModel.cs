namespace NexusForever.Database.Character.Model
{
    public class CharacterCurrencyModel
    {
        public ulong Id { get; set; }
        public byte CurrencyId { get; set; }
        public ulong Amount { get; set; }

        public CharacterModel Character { get; set; }
    }
}
