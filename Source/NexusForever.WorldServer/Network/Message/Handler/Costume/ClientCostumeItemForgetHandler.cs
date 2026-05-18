using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Costume;

namespace NexusForever.WorldServer.Network.Message.Handler.Costume
{
    public class ClientCostumeItemForgetHandler : IMessageHandler<IWorldSession, ClientCostumeItemForget>
    {
        public void HandleMessage(IWorldSession session, ClientCostumeItemForget costumeItemForget)
        {
            session.Player.Account.CostumeManager.ForgetItem(costumeItemForget.Item2Id);
        }
    }
}
