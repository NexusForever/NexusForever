using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Costume
{
    public class ClientCostumeSetHandler : IMessageHandler<IWorldSession, ClientCostumeSet>
    {
        public void HandleMessage(IWorldSession session, ClientCostumeSet costumeSet)
        {
            session.Player.CostumeManager.SetCostume(costumeSet.Index);
        }
    }
}
