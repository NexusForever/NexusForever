using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Utility;

namespace NexusForever.WorldServer.Network.Message.Handler.Entity.Player
{
    public class ClientRequestPlayedHandler : IMessageHandler<IWorldSession, ClientPlayedReqeust>
    {
        public void HandleMessage(IWorldSession session, ClientPlayedReqeust _)
        {
            double diff = session.Player.GetTimeSinceLastSave();
            session.EnqueueMessageEncrypted(new ServerPlayedResponse
            {
                CreateTime        = session.Player.CreateTime,
                TimePlayedSession = (uint)(session.Player.TimePlayedSession + diff),
                TimePlayedTotal   = (uint)(session.Player.TimePlayedTotal + diff),
                TimePlayedLevel   = (uint)(session.Player.TimePlayedLevel + diff)
            });
        }
    }
}
