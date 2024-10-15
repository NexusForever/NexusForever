using System.Numerics;
using NexusForever.Game.Abstract.Map;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IEntitySummonFactory
    {
        public uint SummonCount { get; }

        event Action<uint> OnSummon;
        event Action<uint> OnUnsummon;

        /// <summary>
        /// Initialises the factory with the owner of the summons.
        /// </summary>
        void Initialise(IWorldEntity owner);

        /// <summary>
        /// Summons an entity of <typeparamref name="T"/> at the specified position and rotation and optional callback.
        /// </summary>
        void Summon<T>(IEntitySummonTemplate template, Vector3 position, Vector3 rotation, OnAddDelegate add = null) where T : IWorldEntity;

        /// <summary>
        /// Stop tracking a summon.
        /// </summary>
        /// <remarks>
        /// This should be called when a summon is removed from the world.
        /// </remarks>
        void UntrackSummon(uint guid);

        /// <summary>
        /// Unsummon an summoned entity with supplied guid.
        /// </summary>
        void Unsummon(uint guid);

        /// <summary>
        /// Unsummon all summoned entities.
        /// </summary>
        void Unsummon();
    }
}
