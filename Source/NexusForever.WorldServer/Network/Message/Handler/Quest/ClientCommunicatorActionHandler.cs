using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Quest
{
    public class ClientCommunicatorActionHandler : IMessageHandler<IWorldSession, ClientCommunicatorAction>
    {
        public void HandleMessage(IWorldSession session, ClientCommunicatorAction communicatorAction)
        {
            session.Player.QuestManager.QuestCommunicatorAction((ushort)communicatorAction.QuestId, communicatorAction.Engaged);
        }
    }
}
