namespace NexusForever.Game.Abstract.Housing
{
    public interface IPublicResidence
    {
        ulong ResidenceId { get; init; }
        string Owner { get; init; }
        string Name { get; init; }
    }
}