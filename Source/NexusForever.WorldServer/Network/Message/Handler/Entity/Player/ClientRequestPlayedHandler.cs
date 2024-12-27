using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Entity.Player
{
    public class ClientRequestPlayedHandler : IMessageHandler<IWorldSession, ClientRequestPlayed>
    {
        public void HandleMessage(IWorldSession session, ClientRequestPlayed _)
        {
            double diff = session.Player.GetTimeSinceLastSave();
            session.EnqueueMessageEncrypted(new ServerPlayerPlayed
            {
                CreateTime        = session.Player.CreateTime,
                TimePlayedSession = (uint)(session.Player.TimePlayedSession + diff),
                TimePlayedTotal   = (uint)(session.Player.TimePlayedTotal + diff),
                TimePlayedLevel   = (uint)(session.Player.TimePlayedLevel + diff)
            });
        }
    }
}
