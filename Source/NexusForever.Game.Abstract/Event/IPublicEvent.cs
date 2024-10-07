using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Static.Event;
using NexusForever.Game.Static.Matching;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Event
{
    public interface IPublicEvent : IDisposable, IUpdate
    {
        Guid Guid { get; }
        uint Id { get; }
        bool IsFinalised { get; }
        bool IsBusy { get; set; }
        uint Phase { get; }

        /// <summary>
        /// Map the <see cref="IPublicEvent"/> is on.
        /// </summary>
        IBaseMap Map { get; }

        /// <summary>
        /// Initialise the <see cref="IPublicEvent"/> with the supplied <see cref="IPublicEventManager"/>, <see cref="IPublicEventTemplate"/> and <see cref="IBaseMap"/>.
        /// </summary>
        void Initialise(IPublicEventManager manager, IPublicEventTemplate template, IBaseMap map);

        /// <summary>
        /// Return a collection of all <see cref="IPublicEventTeam"/>'s participating in the <see cref="IPublicEvent"/>.
        /// </summary>
        IEnumerable<IPublicEventTeam> GetTeams();

        /// <summary>
        /// Set the current phase of the <see cref="IPublicEvent"/>.
        /// </summary>
        /// <remarks>
        /// This will spawn any entities that are part of the phase.
        /// </remarks>
        void SetPhase<T>(T phase) where T : Enum;

        /// <summary>
        /// Set the current phase of the <see cref="IPublicEvent"/>.
        /// </summary>
        /// <remarks>
        /// This will spawn any entities that are part of the phase.
        /// </remarks>
        void SetPhase(uint phase);

        /// <summary>
        /// Add <see cref="IPlayer"/> to the <see cref="IPublicEvent"/> on <see cref="PublicEventTeam"/>.
        /// </summary>
        void JoinEvent(IPlayer player, PublicEventTeam team);

        /// <summary>
        /// Remove <see cref="IPlayer"/> from the <see cref="IPublicEvent"/> with the supplied reason.
        /// </summary>
        void LeaveEvent(IPlayer player, PublicEventRemoveReason reason);

        /// <summary>
        /// Invoked when the <see cref="IPvpMatch"/> changes <see cref="PvpGameState"/> for the map the public event is part of.
        /// </summary>
        void OnPvpMatchState(PvpGameState state);

        /// <summary>
        /// Update any objective for the team the <see cref="IPlayer"/> is on that meets the supplied type, objectId and count.
        /// </summary>
        void UpdateObjective(IPlayer player, PublicEventObjectiveType type, uint objectId, int count);

        /// <summary>
        /// Update any objective for any team that meets the supplied <see cref="PublicEventObjectiveType"/>, objectId and count.
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
        /// Activate objective with the supplied objectiveId and max count.
        /// </summary>
        /// <remarks>
        /// Max value is used for dynamic objectives where the max count can change during the event.
        /// </remarks>
        void ActivateObjective<T>(T objectiveId, uint max = 0u) where T : Enum;

        /// <summary>
        /// Activate objective with the supplied objectiveId and max count.
        /// </summary>
        /// <remarks>
        /// Max value is used for dynamic objectives where the max count can change during the event.
        /// </remarks>
        void ActivateObjective(uint objectiveId, uint max = 0u);

        /// <summary>
        /// Reset objective with supplied objectiveId.
        /// </summary>
        void ResetObjective<T>(T objectiveId) where T : Enum;

        /// <summary>
        /// Reset objective with supplied objectiveId.
        /// </summary>
        void ResetObjective(uint objectiveId);

        /// <summary>
        /// Update stat for the <see cref="IPlayer"/> with the supplied <see cref="PublicEventStat"/> and value.
        /// </summary>
        void UpdateStat(IPlayer player, PublicEventStat stat, uint value);

        /// <summary>
        /// Update custom stat for the <see cref="IPlayer"/> with the supplied index and value.
        /// </summary>
        void UpdateCustomStat(IPlayer player, uint index, uint value);

        /// <summary>
        /// Start a vote for <see cref="Static.Event.PublicEventTeam"/> with the supplied voteId and default choice.
        /// </summary>
        /// <remarks>
        /// Default choice will be selected if no response is received within the vote duration.
        /// </remarks>
        void StartVote(PublicEventTeam team, uint voteId, uint defaultChoice);

        /// <summary>
        /// Respond to vote for the <see cref="IPlayer"/> with the supplied choice.
        /// </summary>
        void RespondVote(IPlayer player, uint choice);

        /// <summary>
        /// Finish <see cref="IPublicEvent"/> with the supplied <see cref="PublicEventTeam"/> as the winner.
        /// </summary>
        void Finish(PublicEventTeam? publicEventTeam);

        /// <summary>
        /// Create a new <see cref="IGridEntity"/> that belongs to the <see cref="IPublicEvent"/>.
        /// </summary>
        /// <remarks>
        /// Entity will be automatically removed when the <see cref="IPublicEvent"/> is finished.
        /// </remarks>
        T CreateEntity<T>() where T : IGridEntity;

        /// <summary>
        /// Invoke <see cref="Action{T}"/> against <see cref="IPublicEvent"/> script collection.
        /// </summary>
        void InvokeScriptCollection<T>(Action<T> action);
    }
}
