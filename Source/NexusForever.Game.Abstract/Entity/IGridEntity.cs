﻿using System.Numerics;
using NexusForever.Game.Abstract.Map;
using NexusForever.GameTable.Model;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Entity
{
    /// <summary>
    /// An <see cref="IGridEntity"/> is en entity that is part of an <see cref="IMap"/> grid.
    /// </summary>
    public interface IGridEntity : IUpdate
    {
        uint Guid { get; }
        IBaseMap Map { get; }
        WorldZoneEntry Zone { get; }
        Vector3 Position { get; }

        IMapInfo PreviousMap { get; }

        /// <summary>
        /// Distance between <see cref="IGridEntity"/> and a <see cref="IMapGrid"/> for activation.
        /// </summary>
        float ActivationRange { get; }

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
        /// Return visible <see cref="IWorldEntity"/> by supplied guid.
        /// </summary>
        T GetVisible<T>(uint guid) where T : IGridEntity;

        /// <summary>
        /// Return visible <see cref="IWorldEntity"/> by supplied creature id.
        /// </summary>
        IEnumerable<T> GetVisibleCreature<T>(uint creatureId) where T : IWorldEntity;
    }
}