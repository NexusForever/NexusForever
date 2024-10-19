using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Static.Event;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Event
{
    public interface IPublicEventManager : IUpdate
    {
        /// <summary>
        /// Initialise <see cref="IPublicEventManager"/> with suppled <see cref="IBaseMap"/> owner.
        /// </summary>
        void Initialise(IBaseMap map);

        /// <summary>
        /// Force cleanup of all <see cref="IPublicEvent"/> and <see cref="IPublicEventCharacter"/>.
        /// </summary>
        /// <remarks>
        /// This should really only be called when the map is being unloaded.
        /// </remarks>
        void Cleanup();

        /// <summary>
        /// Create a new <see cref="IPublicEvent"/> with supplied id.
        /// </summary>
        IPublicEvent CreateEvent(uint id);

        /// <summary>
        /// Return the <see cref="IPublicEvent"/> from owner <see cref="IBaseMap"/> with the supplied id.
        /// </summary>
        IPublicEvent GetEvent(uint id);

        /// <summary>
        /// Return a collection of all <see cref="IPublicEvent"/>'s on the owner <see cref="IBaseMap"/>.
        /// </summary>
        IEnumerable<IPublicEvent> GetEvents();

        /// <summary>
        /// Add character to <see cref="IPublicEvent"/>.
        /// </summary>
        void AddEvent(ulong characterId, IPublicEvent publicEvent);

        /// <summary>
        /// Remove character from <see cref="IPublicEvent"/>.
        /// </summary>
        void RemoveEvent(ulong characterId, IPublicEvent publicEvent);

        /// <summary>
        /// Invoked when a <see cref="IGridEntity"/> is added to owner <see cref="IBaseMap"/>.
        /// </summary>
        void OnAddToMap(IGridEntity gridEntity);

        /// <summary>
        /// Invoked when a <see cref="IGridEntity"/> is removed from owner <see cref="IBaseMap"/>.
        /// </summary>
        void OnRemoveFromMap(IGridEntity gridEntity);

        /// <summary>
        /// Update any objective for any public event <see cref="IPlayer"/> is part of that meets the supplied <see cref="PublicEventObjectiveType"/>, objectId and count.
        /// </summary>
        void UpdateObjective(IPlayer player, PublicEventObjectiveType type, uint objectId, int count);

        /// <summary>
        /// Update any objective for any public event that meets the supplied <see cref="PublicEventObjectiveType"/>, objectId and count.
        /// </summary>
        void UpdateObjective(PublicEventObjectiveType type, uint objectId, int count);

        /// <summary>
        /// Update a specific objective with the supplied objectiveId and count.
        /// </summary>
        void UpdateObjective<T>(T objectiveId, int count) where T : Enum;

        /// <summary>
        /// Update a specific objective with the supplied objectiveId and count.
        /// </summary>
        void UpdateObjective(uint objectiveId, int count);

        /// <summary>
        /// Update stat for any public event <see cref="IPlayer"/> is part of with the supplied <see cref="PublicEventStat"/> and value.
        /// </summary>
        void UpdateStat(IPlayer player, PublicEventStat stat, uint value);

        /// <summary>
        /// Update custom stat for any public event <see cref="IPlayer"/> is part of with the supplied index and value.
        /// </summary>
        void UpdateCustomStat(IPlayer player, uint index, uint value);

        /// <summary>
        /// Respond to vote in a specific public event for the <see cref="IPlayer"/> with the supplied choice.
        /// </summary>
        void RespondVote(IPlayer player, uint eventId, uint choice);

        /// <summary>
        /// Invoked when a cinematic finishes for <see cref="IPlayer"/>.
        /// </summary>
        void OnCinematicFinish(IPlayer player, uint cinematicId);
    }
}
