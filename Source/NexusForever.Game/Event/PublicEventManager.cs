using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Event;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Static.Event;
using NexusForever.Script.Template;
using NexusForever.Shared;

namespace NexusForever.Game.Event
{
    public class PublicEventManager : IPublicEventManager
    {
        private IBaseMap map;
        private readonly Dictionary<uint, IPublicEvent> publicEvents = [];

        private readonly Dictionary<ulong, IPublicEventCharacter> characters = [];

        #region Dependency Injection

        private readonly ILogger<PublicEventManager> log;

        private readonly IPublicEventTemplateManager publicEventTemplateManager;
        private readonly IPublicEventFactory publicEventFactory;
        private readonly IFactory<IPublicEventCharacter> publicEventCharacterFactory;

        public PublicEventManager(
            ILogger<PublicEventManager> log,
            IPublicEventTemplateManager publicEventTemplateManager,
            IPublicEventFactory publicEventFactory,
            IFactory<IPublicEventCharacter> publicEventCharacterFactory)
        {
            this.log                         = log;

            this.publicEventTemplateManager  = publicEventTemplateManager;
            this.publicEventFactory          = publicEventFactory;
            this.publicEventCharacterFactory = publicEventCharacterFactory;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="PublicEventManager"/> with suppled <see cref="IBaseMap"/> owner.
        /// </summary>
        public void Initialise(IBaseMap map)
        {
            if (this.map != null)
                throw new InvalidOperationException();

            this.map = map;

            log.LogTrace($"Initialised public event manager for map {map.Entry.Id}.");
        }

        /// <summary>
        /// Force cleanup of all <see cref="IPublicEvent"/> and <see cref="IPublicEventCharacter"/>.
        /// </summary>
        /// <remarks>
        /// This should really only be called when the map is being unloaded.
        /// </remarks>
        public void Cleanup()
        {
            foreach (IPublicEvent @event in publicEvents.Values)
            {
                @event.Finish(null);
                @event.Dispose();
            }

            publicEvents.Clear();
            characters.Clear();
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            if (publicEvents.Count == 0)
                return;

            var toRemove = new List<IPublicEvent>();

            foreach (IPublicEvent @event in publicEvents.Values)
            {
                @event.Update(lastTick);
                if (@event.IsFinalised)
                    toRemove.Add(@event);
            }

            foreach (IPublicEvent @event in toRemove)
            {
                publicEvents.Remove(@event.Id);
                @event.Dispose();

                log.LogTrace($"Removed public event {@event.Id} for map {map.Entry.Id} from store.");
            }
        }

        /// <summary>
        /// Create a new <see cref="IPublicEvent"/> with supplied id.
        /// </summary>
        public IPublicEvent CreateEvent(uint id)
        {
            IPublicEventTemplate template = publicEventTemplateManager.GetTemplate(id);
            if (template == null)
                return null;

            if (template.Entry.WorldId != map.Entry.Id)
                throw new InvalidOperationException($"Public event {id} is not available in map {map.Entry.Id}!");

            IPublicEvent @event = publicEventFactory.CreateEvent(id);
            @event.Initialise(this, template, map);

            log.LogTrace($"Created new public event {id} for map {map.Entry.Id}.");

            publicEvents.Add(id, @event);
            return @event;
        }

        /// <summary>
        /// Return the <see cref="IPublicEvent"/> from owner <see cref="IBaseMap"/> with the supplied id.
        /// </summary>
        public IPublicEvent GetEvent(uint id)
        {
            return publicEvents.TryGetValue(id, out IPublicEvent @event) ? @event : null;
        }

        /// <summary>
        /// Return a collection of all <see cref="IPublicEvent"/>'s on the owner <see cref="IBaseMap"/>.
        /// </summary>
        public IEnumerable<IPublicEvent> GetEvents()
        {
            return publicEvents.Values;
        }

        /// <summary>
        /// Add character to <see cref="IPublicEvent"/>.
        /// </summary>
        public void AddEvent(ulong characterId, IPublicEvent publicEvent)
        {
            if (!characters.TryGetValue(characterId, out IPublicEventCharacter character))
                return;

            character.AddEvent(publicEvent);
        }

        /// <summary>
        /// Remove character from <see cref="IPublicEvent"/>.
        /// </summary>
        public void RemoveEvent(ulong characterId, IPublicEvent publicEvent)
        {
            if (!characters.TryGetValue(characterId, out IPublicEventCharacter character))
                return;

            character.RemoveEvent(publicEvent);
        }

        /// <summary>
        /// Invoked when a <see cref="IGridEntity"/> is added to owner <see cref="IBaseMap"/>.
        /// </summary>
        public void OnAddToMap(IGridEntity gridEntity)
        {
            if (gridEntity is IPlayer player)
            {
                IPublicEventCharacter character = publicEventCharacterFactory.Resolve();
                character.Initialise(player);

                characters.TryAdd(player.CharacterId, character);

                log.LogTrace($"Public event information for character {player.CharacterId} added to map {map.Entry.Id} store.");
            }

            InvokeScriptCollection<IPublicEventScript>(s => s.OnAddToMap(gridEntity));
        }

        /// <summary>
        /// Invoked when a <see cref="IGridEntity"/> is removed from owner <see cref="IBaseMap"/>.
        /// </summary>
        public void OnRemoveFromMap(IGridEntity gridEntity)
        {
            InvokeScriptCollection<IPublicEventScript>(s => s.OnRemoveFromMap(gridEntity));

            if (gridEntity is not IPlayer player)
                return;

            if (!characters.TryGetValue(player.CharacterId, out IPublicEventCharacter character))
                return;

            character.OnRemoveFromMap(player);

            characters.Remove(player.CharacterId);

            log.LogTrace($"Public event information for character {player.CharacterId} removed from map {map.Entry.Id} store.");
        }

        /// <summary>
        /// Update any objective for any public event <see cref="IPlayer"/> is part of that meets the supplied <see cref="PublicEventObjectiveType"/>, objectId and count.
        /// </summary>
        public void UpdateObjective(IPlayer player, PublicEventObjectiveType type, uint objectId, int count)
        {
            if (!characters.TryGetValue(player.CharacterId, out IPublicEventCharacter character))
                return;

            character.UpdateObjective(player, type, objectId, count);
        }

        /// <summary>
        /// Update stat for any public event <see cref="IPlayer"/> is part of with the supplied <see cref="PublicEventStat"/> and value.
        /// </summary>
        public void UpdateStat(IPlayer player, PublicEventStat stat, uint value)
        {
            if (!characters.TryGetValue(player.CharacterId, out IPublicEventCharacter character))
                return;

            character.UpdateStat(player, stat, value);
        }

        /// <summary>
        /// Update custom stat for any public event <see cref="IPlayer"/> is part of with the supplied index and value.
        /// </summary>
        public void UpdateCustomStat(IPlayer player, uint index, uint value)
        {
            if (!characters.TryGetValue(player.CharacterId, out IPublicEventCharacter character))
                return;

            character.UpdateCustomStat(player, index, value);
        }

        /// <summary>
        /// Respond to vote in a specific public event for the <see cref="IPlayer"/> with the supplied choice.
        /// </summary>
        public void RespondVote(IPlayer player, uint eventId, uint choice)
        {
            if (!characters.TryGetValue(player.CharacterId, out IPublicEventCharacter character))
                return;

            character.RespondVote(player, eventId, choice);
        }

        private void InvokeScriptCollection<T>(Action<T> action)
        {
            foreach (IPublicEvent @event in publicEvents.Values)
                @event.InvokeScriptCollection(action);
        }
    }
}
