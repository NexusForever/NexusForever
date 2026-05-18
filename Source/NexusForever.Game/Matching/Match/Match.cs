using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Abstract.Matching;
using NexusForever.Game.Abstract.Matching.Match;
using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Game.Static.Matching;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Match;
using NexusForever.Network.Message;
using NexusForever.Shared;
using NexusForever.Shared.Game;

namespace NexusForever.Game.Matching.Match
{
    public class Match : IMatch
    {
        public Guid Guid { get; private set; }
        public MatchStatus Status { get; private set; }
        public IMatchingMap MatchingMap { get; private set; }

        protected IContentMapInstance map;

        private readonly List<IMatchTeam> teams = [];
        private readonly Dictionary<Identity, IMatchTeam> characterTeams = [];

        private UpdateTimer closeTimer;

        #region Dependency Injection

        protected readonly ILogger log;

        private readonly IMatchManager matchManager;
        private readonly IMatchingDataManager matchingDataManager;
        private readonly IFactory<IMatchTeam> matchTeamFactory;
        private readonly IGameTableManager gameTableManager;
        private readonly IPlayerManager playerManager;
        private readonly IInternalMessagePublisher messagePublisher;

        public Match(
            ILogger<Match> log,
            IMatchManager matchManager,
            IMatchingDataManager matchingDataManager,
            IFactory<IMatchTeam> matchTeamFactory,
            IGameTableManager gameTableManager,
            IPlayerManager playerManager,
            IInternalMessagePublisher messagePublisher)
        {
            this.log                 = log;

            this.matchManager        = matchManager;
            this.matchingDataManager = matchingDataManager;
            this.matchTeamFactory    = matchTeamFactory;
            this.gameTableManager    = gameTableManager;
            this.playerManager       = playerManager;
            this.messagePublisher    = messagePublisher;
        }

        #endregion

        /// <summary>
        /// Initialise the match with the supplied <see cref="IMatchProposal"/>
        /// </summary>
        public virtual void Initialise(IMatchProposal matchProposal)
        {
            if (Guid != Guid.Empty)
                throw new InvalidOperationException();

            Guid        = Guid.NewGuid();
            MatchingMap = matchProposal.MatchingMapSelectorResult.MatchingMap;

            InitialiseTeams(matchProposal);

            messagePublisher.PublishAsync(new MatchCreatedMessage
            {
                Match = this.ToInternalMatch()
            }).FireAndForgetAsync();
        }

        protected virtual void InitialiseTeams(IMatchProposal matchProposal)
        {
            // the red team is always used for PvE matches
            Static.Matching.MatchTeam team = Static.Matching.MatchTeam.Red;
            foreach (IMatchingQueueGroupTeam matchingQueueGroupTeam in matchProposal.MatchingQueueGroup.GetTeams())
            {
                InitialiseTeam(team, matchingQueueGroupTeam);
                team = team == Static.Matching.MatchTeam.Blue ? Static.Matching.MatchTeam.Red : Static.Matching.MatchTeam.Blue;
            }
        }

        protected void InitialiseTeam(Static.Matching.MatchTeam team, IMatchingQueueGroupTeam matchingQueueGroupTeam)
        {
            IMatchTeam matchTeam = matchTeamFactory.Resolve();
            matchTeam.Initialise(this, team);
            teams.Add(matchTeam);

            foreach (IMatchingQueueProposalMember matchingQueueProposalMember in matchingQueueGroupTeam.GetMembers())
                MatchJoin(matchTeam, matchingQueueProposalMember.Identity, matchingQueueProposalMember.Roles);
        }

        private void MatchJoin(IMatchTeam matchTeam, Identity identity, Role roles)
        {
            matchTeam.MatchJoin(identity, roles);
            characterTeams.Add(identity, matchTeam);
            matchManager.GetMatchCharacter(identity).AddMatch(this);

            log.LogTrace($"Added member {identity} to match {Guid}.");
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public virtual void Update(double lastTick)
        {
            if (closeTimer == null)
                return;

            closeTimer.Update(lastTick);
            if (closeTimer.HasElapsed)
            {
                log.LogTrace($"Close timer for match {Guid} has elapsed.");

                closeTimer = null;
                MatchCleanup();
            }
        }

        /// <summary>
        /// Invoked when <see cref="IPlayer"/> logs in.
        /// </summary>
        public void OnLogin(IPlayer player)
        {
            IMatchTeam matchTeam = GetTeam(player.Identity);
            matchTeam.OnLogin(player);
        }

        /// <summary>
        /// Set <see cref="IContentMapInstance"/> the match is running on.
        /// </summary>
        public void SetMap(IContentMapInstance contentMapInstance)
        {
            if (map != null)
                throw new InvalidOperationException();

            map = contentMapInstance;
        }

        /// <summary>
        /// Cleanup <see cref="IMatch"/>, removing all members, detatch map and finalising the match.
        /// </summary>
        public void MatchCleanup()
        {
            log.LogTrace($"Cleaning up match {Guid}...");
            Status = MatchStatus.Cleanup;

            messagePublisher.PublishAsync(new MatchRemovedMessage
            {
                Match = this.ToInternalMatch()
            }).FireAndForgetAsync();

            foreach (IMatchTeam matchTeam in GetTeams())
            {
                foreach (IMatchTeamMember matchTeamMember in matchTeam.GetMembers())
                {
                    log.LogTrace($"Removing member {matchTeamMember.Identity} from match {Guid} due to cleanup.");

                    // it is possible for the player to be offline during cleanup
                    // in this case we just need to remove them from the match, no need to teleport or exit
                    IPlayer player = playerManager.GetPlayer(matchTeamMember.Identity);
                    if (player != null)
                        MatchExit(player, true);

                    MatchLeave(matchTeamMember.Identity);
                }
            }

            map.RemoveMatch();

            Status = MatchStatus.Finalised;

            log.LogTrace($"Match {Guid} has been finalised.");
        }

        /// <summary>
        /// Return <see cref="IMatchTeam"/> for supplied character.
        /// </summary>
        public IMatchTeam GetTeam(Identity identity)
        {
            return characterTeams.TryGetValue(identity, out IMatchTeam team) ? team : null;
        }

        /// <summary>
        /// Return a collection containing <see cref="IMatchTeam"/> in the match.
        /// </summary>
        public IEnumerable<IMatchTeam> GetTeams()
        {
            return teams;
        }

        /// <summary>
        /// Invoked when <see cref="IPlayer"/> enters the match.
        /// </summary>
        public virtual void MatchEnter(IPlayer player)
        {
            IMatchTeam team = GetTeam(player.Identity);
            if (team == null)
                throw new InvalidOperationException();

            team.MatchEnter(player.Identity, MatchingMap);
            player.SetTemporaryFaction(team.Faction);

            log.LogTrace($"Member {player.Identity} has entered match {Guid}.");
        }

        /// <summary>
        /// Invoked when <see cref="IPlayer"/> exits the match.
        /// </summary>
        public void MatchExit(IPlayer player, bool teleport)
        {
            IMatchTeam team = GetTeam(player.Identity);
            if (team == null)
                return;

            team.MatchExit(player.Identity, teleport);
            player.RemoveTemporaryFaction();

            log.LogTrace($"Member {player.Identity} has exited match {Guid}.");

            // certain match types prevent re-entry after exiting
            if (ShouldLeaveMatchOnExit())
                MatchLeave(player.Identity);
        }

        private bool ShouldLeaveMatchOnExit()
        {
            return Status == MatchStatus.Finished
                || !matchingDataManager.CanReEnterMatch(MatchingMap.GameTypeEntry.MatchTypeEnum);
        }

        /// <summary>
        /// Remove character from match.
        /// </summary>
        public void MatchLeave(Identity identity)
        {
            IMatchTeam team = GetTeam(identity);
            if (team == null)
                return;

            team.MatchLeave(identity);
            characterTeams.Remove(identity);

            matchManager.GetMatchCharacter(identity).RemoveMatch();

            log.LogTrace($"Member {identity} has left match {Guid}.");

            // if all members have left the match, we can cleanup early
            if (CanCleanupEarly())
                MatchCleanup();
        }

        private bool CanCleanupEarly()
        {
            return Status != MatchStatus.Cleanup
                && GetTeams()
                    .SelectMany(t => t.GetMembers())
                    .All(m => !m.InMatch);
        }

        /// <summary>
        /// Finish the match.
        /// </summary>
        public void MatchFinish()
        {
            if (Status != MatchStatus.InProgress)
                return;

            Status = MatchStatus.Finished;

            GameFormulaEntry entry = gameTableManager.GameFormula.GetEntry(656);
            closeTimer = new UpdateTimer(TimeSpan.FromMilliseconds(entry?.Dataint0 ?? 300000));

            map.OnMatchFinish();

            log.LogTrace($"Match {Guid} has finished, closing in {closeTimer.Duration} seconds.");
        }

        /// <summary>
        /// Teleport supplied character to the match.
        /// </summary>
        public void MatchTeleport(Identity identity)
        {
            IMatchTeam team = GetTeam(identity);
            if (team == null)
                throw new InvalidOperationException();

            team.MatchTeleport(identity);
        }

        /// <summary>
        /// Get return <see cref="IMapPosition"/> for <see cref="IPlayer"/>.
        /// </summary>
        /// <remarks>
        /// Return position is the position of the player before entering the match.
        /// </remarks>
        public IMapPosition GetReturnPosition(IPlayer player)
        {
            IMatchTeam team = GetTeam(player.Identity);
            return team.GetReturnPosition(player.Identity);
        }

        protected void Broadcast(IWritable message)
        {
            foreach (IMatchTeam party in teams)
                party.Broadcast(message);
        }
    }
}
