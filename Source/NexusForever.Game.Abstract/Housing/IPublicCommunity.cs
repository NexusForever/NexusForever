namespace NexusForever.Game.Abstract.Housing
{
    public interface IPublicCommunity
    {
        ulong NeighbourhoodId { get; init; }
        string Owner { get; init; }
        string Name { get; init; }
    }
}