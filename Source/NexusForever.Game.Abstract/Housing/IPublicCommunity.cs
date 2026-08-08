namespace NexusForever.Game.Abstract.Housing
{
    public interface IPublicCommunity
    {
        Identity GuildIdentity { get; init; }
        string Owner { get; init; }
        string Name { get; init; }
    }
}