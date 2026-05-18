using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Abilities;

namespace NexusForever.WorldServer.Network.Message.Handler.Spell
{
    public class ClientSetStanceHandler : IMessageHandler<IWorldSession, ClientSetStance>
    {
        public void HandleMessage(IWorldSession session, ClientSetStance innateChange)
        {
            // TODO: Validate that index exists and which ability it is

            session.Player.InnateIndex = innateChange.InnateIndex;

            session.EnqueueMessageEncrypted(new ServerStanceChanged
            {
                InnateIndex = session.Player.InnateIndex
            });
        }
    }
}
