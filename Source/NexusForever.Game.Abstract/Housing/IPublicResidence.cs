namespace NexusForever.Game.Abstract.Housing
{
    public interface IPublicResidence
    {
        Identity Identity { get; init; }
        string Owner { get; init; }
        string Name { get; init; }
    }
}