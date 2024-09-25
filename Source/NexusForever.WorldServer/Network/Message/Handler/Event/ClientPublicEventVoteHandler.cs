using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Event
{
    public class ClientPublicEventVoteHandler : IMessageHandler<IWorldSession, ClientPublicEventVote>
    {
        public void HandleMessage(IWorldSession session, ClientPublicEventVote publicEventVote)
        {
            session.Player.Map.PublicEventManager.RespondVote(session.Player, publicEventVote.EventId, publicEventVote.Choice);
        }
    }
}
