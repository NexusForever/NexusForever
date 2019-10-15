namespace NexusForever.Database.Character.Model
{
    public class CharacterEntitlementModel
    {
        public ulong Id { get; set; }
        public byte EntitlementId { get; set; }
        public uint Amount { get; set; }

        public CharacterModel Character { get; set; }
    }
}
