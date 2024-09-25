using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Event;
using NexusForever.Game.Static.Event;

namespace NexusForever.Game.Event
{
    public class PublicEventCharacter : IPublicEventCharacter
    {
        public ulong CharacterId { get; private set; }

        private readonly Dictionary<uint, IPublicEvent> events = [];

        #region Dependency Injection

        private readonly ILogger<PublicEventCharacter> log;

        public PublicEventCharacter(
            ILogger<PublicEventCharacter> log)
        {
            this.log = log;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="PublicEventCharacter"/> information for <see cref="IPlayer"/>.
        /// </summary>
        public void Initialise(IPlayer player)
        {
            if (CharacterId != 0ul)
                throw new InvalidOperationException($"Public event information for character {CharacterId} is already initialised!");

            CharacterId = player.CharacterId;

            log.LogTrace($"Public event information initialised for character {CharacterId}.");
        }

        /// <summary>
        /// Start tracking a new <see cref="IPublicEvent"/> for character.
        /// </summary>
        public void AddEvent(IPublicEvent publicEvent)
        {
            if (!events.TryAdd(publicEvent.Id, publicEvent))
                throw new InvalidOperationException($"Character {CharacterId} is already participating in event {publicEvent.Guid}!");

            log.LogTrace($"Public event {publicEvent.Guid} added for character {CharacterId}.");
        }

        /// <summary>
        /// Stop tracking a <see cref="IPublicEvent"/> for character.
        /// </summary>
        public void RemoveEvent(IPublicEvent publicEvent)
        {
            if (!events.Remove(publicEvent.Id))
                throw new InvalidOperationException($"Character {CharacterId} is not participating in event {publicEvent.Guid}!");

            log.LogTrace($"Public event {publicEvent.Guid} removed for character {CharacterId}.");
        }

        /// <summary>
        /// Invoked when <see cref="IPlayer"/> is removed from the map.
        /// </summary>
        public void OnRemoveFromMap(IPlayer player)
        {
            foreach (IPublicEvent @event in events.Values)
                @event.LeaveEvent(player, PublicEventRemoveReason.LeftArea);
        }

        /// <summary>
        /// Update any objective for any public event <see cref="IPlayer"/> is part of that meets the supplied <see cref="PublicEventObjectiveType"/>, objectId and count.
        /// </summary>
        public void UpdateObjective(IPlayer player, PublicEventObjectiveType type, uint objectId, int count)
        {
            foreach (IPublicEvent @event in events.Values)
                @event.UpdateObjective(player, type, objectId, count);
        }

        /// <summary>
        /// Update stat for any public event <see cref="IPlayer"/> is part of with the supplied <see cref="PublicEventStat"/> and value.
        /// </summary>
        public void UpdateStat(IPlayer player, PublicEventStat stat, uint value)
        {
            foreach (IPublicEvent @event in events.Values)
                @event.UpdateStat(player, stat, value);
        }

        /// <summary>
        /// Update custom stat for any public event <see cref="IPlayer"/> is part of with the supplied index and value.
        /// </summary>
        public void UpdateCustomStat(IPlayer player, uint index, uint value)
        {
            foreach (IPublicEvent @event in events.Values)
                @event.UpdateCustomStat(player, index, value);
        }

        /// <summary>
        /// Respond to vote in a specific public event for the <see cref="IPlayer"/> with the supplied choice.
        /// </summary>
        public void RespondVote(IPlayer player, uint publicEventId, uint choice)
        {
            if (!events.TryGetValue(publicEventId, out IPublicEvent @event))
                return;

            @event.RespondVote(player, choice);
        }
    }
}
