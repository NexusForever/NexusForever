using System;
using NexusForever.Game.Abstract.Matching.Match;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Matching
{
    public class ClientMatchingMatchLeaveHandler : IMessageHandler<IWorldSession, ClientMatchingMatchLeave>
    {
        #region Dependency Injection

        private readonly IMatchManager matchManager;

        public ClientMatchingMatchLeaveHandler(
            IMatchManager matchManager)
        {
            this.matchManager = matchManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientMatchingMatchLeave _)
        {
            IMatchCharacter matchCharacter = matchManager.GetMatchCharacter(session.Player.CharacterId);

            // deliberately check match null twice as it is possible for MatchLeave to be called from MatchExit which will nullify the match
            matchCharacter.Match?.MatchExit(session.Player, true);
            matchCharacter.Match?.MatchLeave(session.Player.CharacterId);
        }
    }
}
