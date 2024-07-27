using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Matching
{
    public class ClientMatchingRoleCheckResponseHandler : IMessageHandler<IWorldSession, ClientMatchingRoleCheckResponse>
    {
        #region Dependency Injection

        private readonly IMatchingManager matchingManager;

        public ClientMatchingRoleCheckResponseHandler(
            IMatchingManager matchingManager)
        {
            this.matchingManager = matchingManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientMatchingRoleCheckResponse matchingRoleCheckResponse)
        {
            IMatchingRoleCheck matchingRoleCheck = matchingManager.GetMatchingRoleCheck(session.Player.CharacterId);
            if (matchingRoleCheck == null)
            {
                session.EnqueueMessageEncrypted(new ServerMatchingQueueResult
                {
                    Result = Game.Static.Matching.MatchingQueueResult.NotConfirmingRole
                });
                return;
            }

            matchingRoleCheck.Respond(session.Player.CharacterId, matchingRoleCheckResponse.Roles);
        }
    }
}
