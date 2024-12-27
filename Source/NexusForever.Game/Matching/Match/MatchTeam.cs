using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Matching;
using NexusForever.Game.Abstract.Matching.Match;
using NexusForever.Game.Static.Reputation;
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

        private readonly Dictionary<ulong, IMatchTeamMember> members = [];
        private IMapEntrance mapEntrance;

        #region Dependency Injection

        private readonly IMatchingDataManager matchingDataManager;
        private readonly IFactory<IMatchTeamMember> matchTeamMemberFactory;

        public MatchTeam(
            IMatchingDataManager matchingDataManager,
            IFactory<IMatchTeamMember> matchTeamMemberFactory)
        {
            this.matchingDataManager    = matchingDataManager;
            this.matchTeamMemberFactory = matchTeamMemberFactory;
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
        public IMatchTeamMember GetMember(ulong characterId)
        {
            return members.TryGetValue(characterId, out IMatchTeamMember matchTeamMember) ? matchTeamMember : null;
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
            IMatchTeamMember matchTeamMember = GetMember(player.CharacterId);
            SendMatchJoin(matchTeamMember);
        }

        /// <summary>
        /// Add character to team.
        /// </summary>
        public void MatchJoin(ulong characterId)
        {
            if (members.ContainsKey(characterId))
                throw new InvalidOperationException();

            IMatchTeamMember matchTeamMember = matchTeamMemberFactory.Resolve();
            matchTeamMember.Initialise(characterId);
            matchTeamMember.TeleportToMatch(mapEntrance);

            members.Add(characterId, matchTeamMember);

            SendMatchJoin(matchTeamMember);
        }

        private void SendMatchJoin(IMatchTeamMember matchTeamMember)
        {
            matchTeamMember.Send(new ServerMatchingMatchJoined
            {
                MatchingGameMap = match.MatchingMap.Id
            });
        }

        /// <summary>
        /// Invoked when character enters the match.
        /// </summary>
        public void MatchEnter(ulong characterId, IMatchingMap matchingMap)
        {
            IMatchTeamMember matchTeamMember = GetMember(characterId);
            matchTeamMember?.MatchEnter(matchingMap);
        }

        /// <summary>
        /// Invoked when character exist the match.
        /// </summary>
        public void MatchExit(ulong characterId, bool teleport)
        {
            IMatchTeamMember matchTeamMember = GetMember(characterId);
            matchTeamMember?.MatchExit(teleport);
        }

        /// <summary>
        /// Invoked when character leaves the match.
        /// </summary>
        public void MatchLeave(ulong characterId)
        {
            IMatchTeamMember matchTeamMember = GetMember(characterId);
            matchTeamMember?.Send(new ServerMatchingMatchLeft());

            members.Remove(characterId);
        }

        /// <summary>
        /// Teleport character to match.
        /// </summary>
        public void MatchTeleport(ulong characterId)
        {
            IMatchTeamMember matchTeamMember = GetMember(characterId);
            matchTeamMember?.TeleportToMatch(mapEntrance);
        }

        /// <summary>
        /// Get return <see cref="IMapPosition"/> for character.
        /// </summary>
        /// <remarks>
        /// Return position is the position of the character before entering the match.
        /// </remarks>
        public IMapPosition GetReturnPosition(ulong characterId)
        {
            IMatchTeamMember matchTeamMember = GetMember(characterId);
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
