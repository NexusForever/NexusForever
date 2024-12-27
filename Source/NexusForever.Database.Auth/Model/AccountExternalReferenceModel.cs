namespace NexusForever.Database.Auth.Model
{
    public class AccountExternalReferenceModel
    {
        public uint Id { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }

        public AccountModel Account { get; set; }
    }
}
