using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.GalacticArchive;

namespace NexusForever.WorldServer.Network.Message.Handler.GalacticArchive
{
    public class ClientGalacticArchiveViewedHandler : IMessageHandler<IWorldSession, ClientGalacticArchiveViewed>
    {
        public void HandleMessage(IWorldSession session, ClientGalacticArchiveViewed request)
        {
            session.Player.GalacticArchiveManager.ViewArticle(request.ArchiveArticleId);
        }
    }
}
