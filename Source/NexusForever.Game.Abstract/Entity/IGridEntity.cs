using System.Numerics;
using NexusForever.Game.Abstract.Map;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Entity
{
    /// <summary>
    /// An <see cref="IGridEntity"/> is en entity that is part of an <see cref="IMap"/> grid.
    /// </summary>
    public interface IGridEntity : IDisposable, IUpdate
    {
        uint Guid { get; }
        IBaseMap Map { get; }
        Vector3 Position { get; }

        IMapInfo PreviousMap { get; }

        /// <summary>
        /// Determines if the <see cref="IGridEntity"/> is on a <see cref="IBaseMap"/>.
        /// </summary>
        bool InWorld { get; }

        /// <summary>
        /// Distance between <see cref="IGridEntity"/> and a <see cref="IMapGrid"/> for activation.
        /// </summary>
        float ActivationRange { get; }

        float? RangeCheck { get; }

        /// <summary>
        /// Invoke <see cref="Action{T}"/> against <see cref="IGridEntity"/> script collection.
        /// </summary>
        void InvokeScriptCollection<T>(Action<T> action);

        /// <summary>
        /// Enqueue <see cref="IGridEntity"/> for removal from the <see cref="IBaseMap"/>.
        /// </summary>
        void RemoveFromMap();

        /// <summary>
        /// Enqueue <see cref="IGridEntity"/> for relocation on the <see cref="IBaseMap"/>.
        /// </summary>
        void Relocate(Vector3 position);

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is enqueued to be added to <see cref="IBaseMap"/>.
        /// </summary>
        void OnEnqueueAddToMap();

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is added to <see cref="IBaseMap"/>.
        /// </summary>
        void OnAddToMap(IBaseMap map, uint guid, Vector3 vector);

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is enqueued to be removed from <see cref="IBaseMap"/>.
        /// </summary>
        void OnEnqueueRemoveFromMap();

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is removed from <see cref="IBaseMap"/>.
        /// </summary>
        void OnRemoveFromMap();

        /// <summary>
        /// Invoked when <see cref="IGridEntity"/> is relocated.
        /// </summary>
        void OnRelocate(Vector3 vector);

        /// <summary>
        /// Returns if <see cref="IGridEntity"/> can see supplied <see cref="IGridEntity"/>.
        /// </summary>
        bool CanSeeEntity(IGridEntity entity);

        /// <summary>
        /// Add tracked <see cref="IGridEntity"/> that is in vision range.
        /// </summary>
        void AddVisible(IGridEntity entity);

        /// <summary>
        /// Remove tracked <see cref="IGridEntity"/> that is no longer in vision range.
        /// </summary>
        void RemoveVisible(IGridEntity entity);

        /// <summary>
        /// Return visible <see cref="IGridEntity"/> by supplied guid.
        /// </summary>
        T GetVisible<T>(uint guid) where T : IGridEntity;

        /// <summary>
        /// Return visible <see cref="IWorldEntity"/> by supplied creature id.
        /// </summary>
        IEnumerable<T> GetVisibleCreature<T>(uint creatureId) where T : IWorldEntity;

        /// <summary>
        /// Set range check for <see cref="IGridEntity"/>.
        /// </summary>
        void SetInRangeCheck(float range);

        /// <summary>
        /// Checks if the provided <see cref="IGridEntity"/> is at a range to trigger an event on this <see cref="IGridEntity"/>.
        /// </summary>
        void CheckEntityInRange(IGridEntity target);

        /// <summary>
        /// Returns all <see cref="IGridEntity"/> in range of this <see cref="IGridEntity"/>.
        /// </summary>
        IEnumerable<T> GetInRange<T>(uint guid) where T : IGridEntity;
    }
}