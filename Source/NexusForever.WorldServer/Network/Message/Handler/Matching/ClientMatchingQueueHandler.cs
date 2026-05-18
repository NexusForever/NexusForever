using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Matching
{
    public class ClientMatchingQueueHandler : IMessageHandler<IWorldSession, ClientMatchingQueue>
    {
        #region Dependency Injection

        private readonly IMatchingManager matchingManager;

        public ClientMatchingQueueHandler(
            IMatchingManager matchingManager)
        {
            this.matchingManager = matchingManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientMatchingQueue matchingQueue)
        {
            matchingManager.JoinQueue(session.Player, matchingQueue.Roles, matchingQueue.MapData.MatchType,
                matchingQueue.MapData.Maps, matchingQueue.MapData.MatchingGameTypeId, matchingQueue.MapData.QueueFlags);
        }
    }
}
