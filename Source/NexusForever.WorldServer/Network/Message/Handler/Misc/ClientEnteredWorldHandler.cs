using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Misc
{
    public class ClientEnteredWorldHandler : IMessageHandler<IWorldSession, ClientEnteredWorld>
    {
        /// <summary>
        /// Client sends this when it has received everything it needs to leave the loading screen.
        /// For housing maps, this also includes things such as residences and plots.
        /// See 0x732990 in the client for more information.
        /// </summary>
        public void HandleMessage(IWorldSession session, ClientEnteredWorld enteredWorld)
        {
            if (!session.Player.IsLoading)
                throw new InvalidPacketValueException();

            session.Player.OnEnteredWorld();
        }
    }
}
