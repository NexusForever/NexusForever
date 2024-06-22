using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Spell
{
    public class ClientInnateChangeHandler : IMessageHandler<IWorldSession, ClientInnateChange>
    {
        public void HandleMessage(IWorldSession session, ClientInnateChange innateChange)
        {
            // TODO: Validate that index exists and which ability it is

            session.Player.InnateIndex = innateChange.InnateIndex;

            session.EnqueueMessageEncrypted(new ServerPlayerInnate
            {
                InnateIndex = session.Player.InnateIndex
            });
        }
    }
}
