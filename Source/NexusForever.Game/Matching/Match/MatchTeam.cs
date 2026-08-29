using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Matching;
using NexusForever.Game.Abstract.Matching.Match;
using NexusForever.Game.Static.Matching;
using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Match;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared;

namespace NexusForever.Game.Matching.Match
{
    public class MatchTeam : IMatchTeam
    {
        public Static.Matching.MatchTeam Team { get; private set; }

        /// <summary>
        /// The temporary secondary <see cref="Static.Reputation.Faction"/> for members during the match.
        /// </summary>
        /// <remarks>
        /// This is a special faction used only during matches, see <see cref="Faction.MatchingTeam1"/> and <see cref="Faction.MatchingTeam2"/>.
        /// </remarks>
        public Faction Faction { get; private set; }

        private IMatch match;

        private readonly Dictionary<Identity, IMatchTeamMember> members = [];
        private IMapEntrance mapEntrance;

        #region Dependency Injection

        private readonly IMatchingDataManager matchingDataManager;
        private readonly IFactory<IMatchTeamMember> matchTeamMemberFactory;
        private readonly IInternalMessagePublisher messagePublisher;

        public MatchTeam(
            IMatchingDataManager matchingDataManager,
            IFactory<IMatchTeamMember> matchTeamMemberFactory,
            IInternalMessagePublisher messagePublisher)
        {
            this.matchingDataManager    = matchingDataManager;
            this.matchTeamMemberFactory = matchTeamMemberFactory;
            this.messagePublisher       = messagePublisher;
        }

        #endregion

        /// <summary>
        /// Initialise new <see cref="IMatchTeam"/> with supplied <see cref="Static.Matching.MatchTeam"/>.
        /// </summary>
        public void Initialise(IMatch match, Static.Matching.MatchTeam team)
        {
            if (mapEntrance != null)
                throw new InvalidOperationException();

            Team    = team;
            Faction = team == Static.Matching.MatchTeam.Red ? Faction.MatchingTeam1 : Faction.MatchingTeam2;

            this.match  = match;
            mapEntrance = matchingDataManager.GetMapEntrance(match.MatchingMap.GameMapEntry.WorldId, (byte)team);
        }

        /// <summary>
        /// Return <see cref="IMatchTeamMember"/> for supplied characterId.
        /// </summary>
        public IMatchTeamMember GetMember(Identity identity)
        {
            return members.TryGetValue(identity, out IMatchTeamMember matchTeamMember) ? matchTeamMember : null;
        }

        /// <summary>
        /// Return collection of all <see cref="IMatchTeamMember"/>'s in <see cref="IMatchTeam"/>.
        /// </summary>
        public IEnumerable<IMatchTeamMember> GetMembers()
        {
            return members.Values;
        }

        /// <summary>
        /// Invoked when <see cref="IPlayer"/> logs in.
        /// </summary>
        public void OnLogin(IPlayer player)
        {
            IMatchTeamMember matchTeamMember = GetMember(player.Identity);
            SendMatchJoin(matchTeamMember);
        }

        /// <summary>
        /// Add character to team.
        /// </summary>
        public void MatchJoin(Identity identity, Role roles)
        {
            if (members.ContainsKey(identity))
                throw new InvalidOperationException();

            IMatchTeamMember matchTeamMember = matchTeamMemberFactory.Resolve();
            matchTeamMember.Initialise(identity, roles);
            matchTeamMember.TeleportToMatch(mapEntrance);

            members.Add(identity, matchTeamMember);

            SendMatchJoin(matchTeamMember);
        }

        private void SendMatchJoin(IMatchTeamMember matchTeamMember)
        {
            matchTeamMember.Send(new ServerMatchingMatchJoined
            {
                MatchingGameMapId = match.MatchingMap.Id
            });
        }

        /// <summary>
        /// Invoked when character enters the match.
        /// </summary>
        public void MatchEnter(Identity identity, IMatchingMap matchingMap)
        {
            IMatchTeamMember matchTeamMember = GetMember(identity);
            matchTeamMember?.MatchEnter(matchingMap);
        }

        /// <summary>
        /// Invoked when character exist the match.
        /// </summary>
        public void MatchExit(Identity identity, bool teleport)
        {
            IMatchTeamMember matchTeamMember = GetMember(identity);
            matchTeamMember?.MatchExit(teleport);
        }

        /// <summary>
        /// Invoked when character leaves the match.
        /// </summary>
        public void MatchLeave(Identity identity)
        {
            IMatchTeamMember matchTeamMember = GetMember(identity);
            if (matchTeamMember != null)
            {
                matchTeamMember.Send(new ServerMatchingMatchLeft());

                messagePublisher.PublishAsync(new MatchMemberLeftMessage
                {
                    Match         = match.ToInternalMatch(),
                    Member = matchTeamMember.ToInternalMatchTeamMember()
                }).FireAndForgetAsync();
            }

            members.Remove(identity);
        }

        /// <summary>
        /// Teleport character to match.
        /// </summary>
        public void MatchTeleport(Identity identity)
        {
            IMatchTeamMember matchTeamMember = GetMember(identity);
            matchTeamMember?.TeleportToMatch(mapEntrance);
        }

        /// <summary>
        /// Get return <see cref="IMapPosition"/> for character.
        /// </summary>
        /// <remarks>
        /// Return position is the position of the character before entering the match.
        /// </remarks>
        public IMapPosition GetReturnPosition(Identity identity)
        {
            IMatchTeamMember matchTeamMember = GetMember(identity);
            return matchTeamMember?.ReturnPosition;
        }

        /// <summary>
        /// Broadcast <see cref="IWritable"/> to all members.
        /// </summary>
        public void Broadcast(IWritable message)
        {
            foreach (IMatchTeamMember matchTeamMember in members.Values)
                matchTeamMember.Send(message);
        }
    }
}
