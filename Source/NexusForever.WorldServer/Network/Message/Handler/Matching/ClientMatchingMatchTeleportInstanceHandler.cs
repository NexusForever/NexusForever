using NexusForever.Game.Abstract.Matching.Match;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Matching
{
    public class ClientMatchingMatchTeleportInstanceHandler : IMessageHandler<IWorldSession, ClientMatchingMatchTeleportInstance>
    {
        #region Dependency Injection

        private readonly IMatchManager matchManager;

        public ClientMatchingMatchTeleportInstanceHandler(
            IMatchManager matchManager)
        {
            this.matchManager = matchManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientMatchingMatchTeleportInstance _)
        {
            IMatchCharacter matchCharacter = matchManager.GetMatchCharacter(session.Player.CharacterId);
            matchCharacter.Match?.MatchTeleport(matchCharacter.CharacterId);
        }
    }
}
