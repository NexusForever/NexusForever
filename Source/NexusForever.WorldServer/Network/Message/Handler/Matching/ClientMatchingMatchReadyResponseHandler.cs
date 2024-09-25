using NexusForever.Game.Abstract.Matching.Match;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Matching
{
    public class ClientMatchingMatchReadyResponseHandler : IMessageHandler<IWorldSession, ClientMatchingGameReadyResponse>
    {
        #region Dependency Injection

        private readonly IMatchManager matchManager;

        public ClientMatchingMatchReadyResponseHandler(
            IMatchManager matchManager)
        {
            this.matchManager = matchManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientMatchingGameReadyResponse matchingGameReadyResponse)
        {
            IMatchProposal matchProposal = matchManager.GetMatchCharacter(session.Player.CharacterId).MatchProposal;
            matchProposal?.Respond(session.Player, matchingGameReadyResponse.Response);
        }
    }
}
