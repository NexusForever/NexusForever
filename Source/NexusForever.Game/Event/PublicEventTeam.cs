using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Event;
using NexusForever.Game.Static.Event;
using NexusForever.GameTable.Model;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Shared;

namespace NexusForever.Game.Event
{
    public class PublicEventTeam : IPublicEventTeam
    {
        public IPublicEvent PublicEvent { get; private set; }
        public Static.Event.PublicEventTeam Team { get; private set; }
        public bool IsFinialised { get; private set; }

        private readonly Dictionary<ulong, IPublicEventTeamMember> members = [];
        private readonly Dictionary<uint, IPublicEventObjective> objectives = [];

        private readonly IPublicEventStats teamStats;

        private IPublicEventVote vote;

        #region Dependency Injection

        private readonly ILogger<PublicEventTeam> log;

        private readonly IFactory<IPublicEventObjective> objectiveFactory;
        private readonly IFactory<IPublicEventTeamMember> memberFactory;
        private readonly IFactory<IPublicEventVote> voteFactory;

        public PublicEventTeam(
            ILogger<PublicEventTeam> log,
            IFactory<IPublicEventObjective> objectiveFactory,
            IFactory<IPublicEventTeamMember> memberFactory,
            IFactory<IPublicEventVote> voteFactory,
            IPublicEventStats publicEventTeamStats)
        {
            this.log              = log;
            this.objectiveFactory = objectiveFactory;
            this.memberFactory    = memberFactory;
            this.voteFactory      = voteFactory;
            this.teamStats        = publicEventTeamStats;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="PublicEventTeam"/> with the provided <see cref="IPublicEvent"/> owner, <see cref="IPublicEventTemplate"/> and <see cref="PublicEventTeamEntry"/>.
        /// </summary>
        public void Initialise(IPublicEvent publicEvent, IPublicEventTemplate template, PublicEventTeamEntry entry)
        {
            if (PublicEvent != null)
                throw new InvalidOperationException();

            PublicEvent = publicEvent;
            Team        = entry.Id;

            foreach (PublicEventObjectiveEntry objectiveEntry in template.Objectives.Values)
            {
                if (objectiveEntry.PublicEventTeamId != Team)
                    continue;

                IPublicEventObjective objective = objectiveFactory.Resolve();
                objective.Initialise(this, objectiveEntry);
                objectives.Add(objectiveEntry.Id, objective);
            }

            log.LogInformation($"Initialised public event team {Team}.");
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            if (vote != null)
            {
                vote.Update(lastTick);
                if (vote.IsFinalised)
                {
                    log.LogTrace($"Vote {vote.VoteId} for team {Team} has been finalised.");
                    vote = null;
                }
            }

            foreach (IPublicEventObjective objective in objectives.Values)
                objective.Update(lastTick);
        }

        /// <summary>
        /// Return collection of <see cref="IPublicEventObjective"/>'s for this <see cref="IPublicEventTeam"/>.
        /// </summary>
        public IEnumerable<IPublicEventObjective> GetObjectives()
        {
            return objectives.Values;
        }

        /// <summary>
        /// Return collection of <see cref="IPublicEventObjective"/>'s for this <see cref="IPublicEventTeam"/> which should be sent to members of the other team.
        /// </summary>
        public IEnumerable<IPublicEventObjective> GetOtherObjectives()
        {
            // is there a better way to determine this?
            return GetObjectives().Where(o => o.Entry.LocalizedTextIdOtherTeam != 0);
        }

        /// <summary>
        /// Return collection of <see cref="IPublicEventTeamMember"/>'s for this <see cref="IPublicEventTeam"/>.
        /// </summary>
        public IEnumerable<IPublicEventTeamMember> GetMembers()
        {
            return members.Values;
        }

        /// <summary>
        /// Add <see cref="IPlayer"/> to this <see cref="IPublicEventTeam"/>.
        /// </summary>
        public void JoinTeam(IPlayer player)
        {
            IPublicEventTeamMember member = memberFactory.Resolve();
            member.Initialise(player);
            members.Add(player.CharacterId, member);
        }

        /// <summary>
        /// Remove character from this <see cref="IPublicEventTeam"/>.
        /// </summary>
        public void LeaveTeam(ulong characterId)
        {
            members.Remove(characterId);
        }

        /// <summary>
        /// Update any objective for the team that meets the supplied <see cref="PublicEventObjectiveType"/>, objectId and count.
        /// </summary>
        public void UpdateObjective(PublicEventObjectiveType type, uint objectId, int count)
        {
            foreach (IPublicEventObjective objective in objectives.Values)
            {
                if (objective.Status != PublicEventStatus.Active)
                    continue;

                if (objective.Entry.PublicEventObjectiveTypeEnum != type
                    && objective.Entry.ObjectId != objectId)
                    continue;

                objective.UpdateObjective(count);

                log.LogTrace($"Updated public event team {Team} objective {objective.Entry.Id} with count {count}.");
            }

            IsFinialised = RequiredObjectivesCompleted();
        }

        private bool RequiredObjectivesCompleted()
        {
            return objectives.Values
                .Where(o => o.Entry.PublicEventObjectiveCategoryEnum == PublicEventObjectiveCategory.Main)
                .All(o => o.Status == PublicEventStatus.Succeeded);
        }

        /// <summary>
        /// Update specific objective for the team with the supplied count.
        /// </summary>
        public void UpdateObjective(uint objectiveId, int count)
        {
            if (!objectives.TryGetValue(objectiveId, out IPublicEventObjective objective))
                return;

            objective.UpdateObjective(count);
        }

        /// <summary>
        /// Activate specific objective for the team with the supplied max.
        /// </summary>
        /// <remarks>
        /// Max value is used for dynamic objectives where the max count can change during the event.
        /// </remarks>
        public void ActivateObjective(uint objectiveId, uint max)
        {
            if (!objectives.TryGetValue(objectiveId, out IPublicEventObjective objective))
                return;

            objective.DynamicMax = max;
            objective.ActivateObjective();
        }

        /// <summary>
        /// Update stat character with the supplied <see cref="PublicEventStat"/> and value.
        /// </summary>
        public void UpdateStat(ulong characterId, PublicEventStat stat, uint value)
        {
            if (!members.TryGetValue(characterId, out IPublicEventTeamMember member))
                return;

            member.UpdateStat(stat, value);
            teamStats.UpdateStat(stat, value);

            log.LogTrace($"Updated public event team {Team} stat {stat} to {value}.");
        }

        /// <summary>
        /// Update custom stat character with the supplied index and value.
        /// </summary>
        public void UpdateCustomStat(ulong characterId, uint index, uint value)
        {
            if (!members.TryGetValue(characterId, out IPublicEventTeamMember member))
                return;

            member.UpdateCustomStat(index, value);
            teamStats.UpdateCustomStat(index, value);

            log.LogTrace($"Updated public event team {Team} custom stat {index} to {value}.");
        }

        /// <summary>
        /// Build <see cref="PublicEventTeamStats"/> for <see cref="IPublicEventTeam"/>.
        /// </summary>
        /// <remarks>
        /// Contains combined stats for all team members in the <see cref="IPublicEventTeam"/>.
        /// </remarks>
        public PublicEventTeamStats BuildTeamStats()
        {
            return new PublicEventTeamStats
            {
                TeamId = Team,
                Stats  = teamStats.Build()
            };
        }

        /// <summary>
        /// Build <see cref="PublicEventParticipantStats"/> for each <see cref="IPublicEventTeamMember"/> in <see cref="IPublicEventTeam"/>.
        /// </summary>
        public IEnumerable<PublicEventParticipantStats> BuildParticipantStats()
        {
            foreach (IPublicEventTeamMember publicEventTeamMember in members.Values)
            {
                PublicEventParticipantStats participantStat = publicEventTeamMember.BuildParticipantStats();
                participantStat.TeamId = Team;
                yield return participantStat;
            }
        }

        /// <summary>
        /// Start a vote with the supplied voteId and default choice.
        /// </summary>
        /// <remarks>
        /// Default choice will be selected if no response is received within the vote duration.
        /// </remarks>
        public void StartVote(uint voteId, uint defaultChoice)
        {
            // this might not be valid, seems the client can handle multiple votes at once?
            if (vote != null)
                throw new InvalidOperationException();

            vote = voteFactory.Resolve();
            vote.Initialise(this, voteId, defaultChoice);

            log.LogTrace($"Vote {vote.VoteId} for team {Team} has been started.");
        }

        /// <summary>
        /// Respond to vote for the <see cref="IPlayer"/> with the supplied choice.
        /// </summary>
        public void RespondVote(IPlayer player, uint choice)
        {
            if (vote == null)
                return;

            vote.Choice(player.CharacterId, choice);
            log.LogTrace($"Vote {vote.VoteId} for team {Team} response {choice} received from character {player.CharacterId}.");

            if (vote.IsFinalised)
            {
                log.LogTrace($"Vote {vote.VoteId} for team {Team} has been finalised.");
                vote = null;
            }    
        }

        /// <summary>
        /// Broadcast <see cref="IWritable"/> to all <see cref="IPublicEventTeamMember"/> in <see cref="IPublicEventTeam"/>.
        /// </summary>
        public void Broadcast(IWritable message)
        {
            foreach (IPublicEventTeamMember member in members.Values)
                member.Send(message);
        }
    }
}
