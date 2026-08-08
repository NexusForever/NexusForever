namespace NexusForever.Server.Character
{
    public record IdentityName
    {
        public required string Name { get; init; }
        public required string RealmName { get; init; }
    }
}
