using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Costume
{
    public class ClientCostumeSaveHandler : IMessageHandler<IWorldSession, ClientCostumeSave>
    {
        public void HandleMessage(IWorldSession session, ClientCostumeSave costumeSave)
        {
            session.Player.CostumeManager.SaveCostume(costumeSave);
        }
    }
}
