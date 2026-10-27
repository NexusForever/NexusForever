using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.GalacticArchive;

namespace NexusForever.WorldServer.Network.Message.Handler.GalacticArchive
{
    public class ClientGalacticArchiveUnlockHandler : IMessageHandler<IWorldSession, ClientGalacticArchiveUnlock>
    {
        /// <summary>
        /// Handle the client unlocking an archive article it has already unlocked locally.
        /// </summary>
        public void HandleMessage(IWorldSession session, ClientGalacticArchiveUnlock request)
        {
            session.Player.GalacticArchiveManager.UnlockArticle(request.ArchiveArticleId);
        }
    }
}
