using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Event;

namespace NexusForever.Game.Abstract.Event
{
    public interface IPublicEventCharacter
    {
        ulong CharacterId { get; }

        /// <summary>
        /// Initialise <see cref="IPublicEventCharacter"/> information for <see cref="IPlayer"/>.
        /// </summary>
        void Initialise(IPlayer player);

        /// <summary>
        /// Start tracking a new <see cref="IPublicEvent"/> for character.
        /// </summary>
        void AddEvent(IPublicEvent publicEvent);

        /// <summary>
        /// Stop tracking a <see cref="IPublicEvent"/> for character.
        /// </summary>
        void RemoveEvent(IPublicEvent publicEvent);

        /// <summary>
        /// Invoked when <see cref="IPlayer"/> is removed from the map.
        /// </summary>
        void OnRemoveFromMap(IPlayer player);

        /// <summary>
        /// Update any objective for any public event <see cref="IPlayer"/> is part of that meets the supplied <see cref="PublicEventObjectiveType"/>, objectId and count.
        /// </summary>
        void UpdateObjective(IPlayer player, PublicEventObjectiveType type, uint objectId, int count);

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
    }
}
