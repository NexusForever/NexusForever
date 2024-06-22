using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Misc
{
    public class ClientZoneChangeHandler : IMessageHandler<IWorldSession, ClientZoneChange>
    {
        public void HandleMessage(IWorldSession session, ClientZoneChange zoneChange)
        {
        }
    }
}
