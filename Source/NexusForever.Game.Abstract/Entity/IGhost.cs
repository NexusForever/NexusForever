namespace NexusForever.Game.Abstract.Entity
{
    public interface IGhost : IWorldEntity
    {
        IPlayer Owner { get; }

        uint GetCostForRez();
    }
}