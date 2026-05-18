using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Matching
{
    public class ClientMatchingQueuePartyHandler : IMessageHandler<IWorldSession, ClientMatchingQueueParty>
    {
        #region Dependency Injection

        private readonly IMatchingManager matchingManager;

        public ClientMatchingQueuePartyHandler(
            IMatchingManager matchingManager)
        {
            this.matchingManager = matchingManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientMatchingQueueParty matchingQueueParty)
        {
            matchingManager.JoinPartyQueue(session.Player, matchingQueueParty.Roles, matchingQueueParty.MapData.MatchType,
                matchingQueueParty.MapData.Maps, matchingQueueParty.MapData.MatchingGameTypeId, matchingQueueParty.MapData.QueueFlags);
        }
    }
}
