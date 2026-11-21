using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Game.Abstract.Loot.Distribute
{
    public interface ILootDistributor
    {
        /// <summary>
        /// Distribute entity loot.
        /// </summary>
        /// <param name="source">Entity to distribute loot from.</param>
        void Distribute(IUnitEntity source);
    }
}
