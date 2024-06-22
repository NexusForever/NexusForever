using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Housing
{
    public class ClientHousingEditModeHandler : IMessageHandler<IWorldSession, ClientHousingEditMode>
    {
        public void HandleMessage(IWorldSession session, ClientHousingEditMode housingEditMode)
        {
        }
    }
}
