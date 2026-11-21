using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Game.Abstract.Loot.Generate
{
    public interface ILootGenerator
    {
        float Probability { get; }
        uint MinCount { get; }
        uint MaxCount { get; }

        /// <summary>
        /// Determines whether this generator can generate loot.
        /// </summary>
        bool CanGenerateLoot();

        /// <summary>
        /// Generates loot and adds it to the specified loot builder.
        /// </summary>
        /// <param name="lootBuilder">The loot builder to add generated loot to.</param>
        /// <param name="players">The players eligible to receive the generated loot.</param>
        void GenerateLoot(ILootBuilder lootBuilder, IEnumerable<IPlayer> players);
    }
}
