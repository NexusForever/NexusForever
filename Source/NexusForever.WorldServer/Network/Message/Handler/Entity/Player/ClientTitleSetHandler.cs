using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Entity.Player
{
    public class ClientTitleSetHandler : IMessageHandler<IWorldSession, ClientTitleSet>
    {
        public void HandleMessage(IWorldSession session, ClientTitleSet request)
        {
            session.Player.TitleManager.ActiveTitleId = request.TitleId;
        }
    }
}
