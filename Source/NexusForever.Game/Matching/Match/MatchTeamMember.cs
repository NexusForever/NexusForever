using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Matching;
using NexusForever.Game.Abstract.Matching.Match;
using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Game.Map;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Matching.Match
{
    public class MatchTeamMember : IMatchTeamMember
    {
        public ulong CharacterId { get; private set; }
        public bool InMatch { get; private set; }
        public IMapPosition ReturnPosition { get; private set; }
        public Vector3 ReturnRotation { get; private set;  }

        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public MatchTeamMember(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        /// <summary>
        /// Initialise new <see cref="IMatchTeamMember"/> with supplied <see cref="IMatchingQueueProposalMember"/>.
        /// </summary>
        public void Initialise(IMatchingQueueProposalMember matchingQueueProposalMember)
        {
            if (CharacterId != 0)
                throw new InvalidOperationException();

            CharacterId = matchingQueueProposalMember.CharacterId;
        }

        /// <summary>
        /// Invoked when member enters the match.
        /// </summary>
        public void MatchEnter(IMatchingMap matchingMap)
        {
            if (InMatch)
                throw new InvalidOperationException();

            InMatch = true;
            Send(new ServerMatchingMatchEntered
            {
                MatchingGameMap = matchingMap.Id
            });
        }

        /// <summary>
        /// Invoked when member exits the match.
        /// </summary>
        public void MatchExit()
        {
            if (!InMatch)
                throw new InvalidOperationException();

            InMatch = false;
            Send(new ServerMatchingMatchExited());
        }

        /// <summary>
        /// Teleport member to match.
        /// </summary>
        public void TeleportToMatch(IMapEntrance mapEntrance)
        {
            IPlayer player = playerManager.GetPlayer(CharacterId);
            if (player == null)
                return;

            ReturnRotation = player.Rotation;
            ReturnPosition = new MapPosition
            {
                Info = new MapInfo
                {
                    Entry = player.Map.Entry
                },
                Position = player.Position
            };

            player.Rotation = mapEntrance.Rotation;
            player.TeleportTo(mapEntrance.MapId, mapEntrance.Position.X, mapEntrance.Position.Y, mapEntrance.Position.Z);
        }

        /// <summary>
        /// Teleport member to return location.
        /// </summary>
        /// <remarks>
        /// Return position is the position of the character before entering the match.
        /// </remarks>
        public void TeleportToReturn()
        {
            IPlayer player = playerManager.GetPlayer(CharacterId);
            if (player == null)
                return;

            player.Rotation = ReturnRotation;
            player.TeleportTo(ReturnPosition);
        }

        /// <summary>
        /// Send <see cref="IWritable"/> to member.
        /// </summary>
        public void Send(IWritable message)
        {
            IPlayer player = playerManager.GetPlayer(CharacterId);
            player?.Session.EnqueueMessageEncrypted(message);
        }
    }
}
