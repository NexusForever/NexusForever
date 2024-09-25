using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Event;
using NexusForever.GameTable.Model;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Event
{
    public interface IPublicEventTeam : IUpdate
    {
        IPublicEvent PublicEvent { get; }
        PublicEventTeam Team { get; }
        bool IsFinialised { get; }

        /// <summary>
        /// Initialise <see cref="IPublicEventTeam"/> with the provided <see cref="IPublicEvent"/> owner, <see cref="IPublicEventTemplate"/> and <see cref="PublicEventTeamEntry"/>.
        /// </summary>
        void Initialise(IPublicEvent publicEvent, IPublicEventTemplate template, PublicEventTeamEntry entry);

        /// <summary>
        /// Return collection of <see cref="IPublicEventObjective"/>'s for this <see cref="IPublicEventTeam"/>.
        /// </summary>
        IEnumerable<IPublicEventObjective> GetObjectives();

        /// <summary>
        /// Return collection of <see cref="IPublicEventObjective"/>'s for this <see cref="IPublicEventTeam"/> which should be sent to members of the other team.
        /// </summary>
        IEnumerable<IPublicEventObjective> GetOtherObjectives();

        /// <summary>
        /// Return collection of <see cref="IPublicEventTeamMember"/>'s for this <see cref="IPublicEventTeam"/>.
        /// </summary>
        IEnumerable<IPublicEventTeamMember> GetMembers();

        /// <summary>
        /// Add <see cref="IPlayer"/> to this <see cref="IPublicEventTeam"/>.
        /// </summary>
        void JoinTeam(IPlayer player);

        /// <summary>
        /// Remove character from this <see cref="IPublicEventTeam"/>.
        /// </summary>
        void LeaveTeam(ulong characterId);

        /// <summary>
        /// Update any objective for the team that meets the supplied <see cref="PublicEventObjectiveType"/>, objectId and count.
        /// </summary>
        void UpdateObjective(PublicEventObjectiveType type, uint objectId, int count);

        /// <summary>
        /// Update specific objective for the team with the supplied count.
        /// </summary>
        void UpdateObjective(uint objectiveId, int count);

        /// <summary>
        /// Activate specific objective for the team with the supplied max.
        /// </summary>
        /// <remarks>
        /// Max value is used for dynamic objectives where the max count can change during the event.
        /// </remarks>
        void ActivateObjective(uint objectiveId, uint max);

        /// <summary>
        /// Update stat character with the supplied <see cref="PublicEventStat"/> and value.
        /// </summary>
        void UpdateStat(ulong characterId, PublicEventStat stat, uint value);

        /// <summary>
        /// Update custom stat character with the supplied index and value.
        /// </summary>
        void UpdateCustomStat(ulong characterId, uint index, uint value);

        /// <summary>
        /// Build <see cref="PublicEventTeamStats"/> for <see cref="IPublicEventTeam"/>.
        /// </summary>
        /// <remarks>
        /// Contains combined stats for all team members in the <see cref="IPublicEventTeam"/>.
        /// </remarks>
        PublicEventTeamStats BuildTeamStats();

        /// <summary>
        /// Build <see cref="PublicEventParticipantStats"/> for each <see cref="IPublicEventTeamMember"/> in <see cref="IPublicEventTeam"/>.
        /// </summary>
        IEnumerable<PublicEventParticipantStats> BuildParticipantStats();

        /// <summary>
        /// Start a vote with the supplied voteId and default choice.
        /// </summary>
        /// <remarks>
        /// Default choice will be selected if no response is received within the vote duration.
        /// </remarks>
        void StartVote(uint voteId, uint defaultChoice);

        /// <summary>
        /// Respond to vote for the <see cref="IPlayer"/> with the supplied choice.
        /// </summary>
        void RespondVote(IPlayer player, uint choice);

        /// <summary>
        /// Broadcast <see cref="IWritable"/> to all <see cref="IPublicEventTeamMember"/> in <see cref="IPublicEventTeam"/>.
        /// </summary>
        void Broadcast(IWritable message);
    }
}
