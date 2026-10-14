using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.GalacticArchive;

namespace NexusForever.WorldServer.Network.Message.Handler.GalacticArchive
{
    public class ClientGalacticArchiveViewedHandler : IMessageHandler<IWorldSession, ClientGalacticArchiveViewed>
    {
        /// <summary>
        /// Handle the client viewing an archive article it has already marked as viewed locally.
        /// </summary>
        public void HandleMessage(IWorldSession session, ClientGalacticArchiveViewed request)
        {
            session.Player.GalacticArchiveManager.ViewArticle(request.ArchiveArticleId);
        }
    }
}
