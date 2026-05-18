using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Entity.Player
{
    public class ClientResurrectAcceptHandler : IMessageHandler<IWorldSession, ClientResurrectAccept>
    {
        public void HandleMessage(IWorldSession session, ClientResurrectAccept clientResurrectAccept)
        {
            if (clientResurrectAccept.RezType == ResurrectionType.None)
                return;

            session.Player.ResurrectionManager.Resurrect(clientResurrectAccept.RezType);
        }
    }
}
