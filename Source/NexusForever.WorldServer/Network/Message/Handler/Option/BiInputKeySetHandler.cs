using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Option;

namespace NexusForever.WorldServer.Network.Message.Handler.Option
{
    public class BiInputKeySetHandler : IMessageHandler<IWorldSession, BiInputKeySet>
    {
        public void HandleMessage(IWorldSession session, BiInputKeySet biInputKeySet)
        {
            if (biInputKeySet.CharacterId != 0ul)
                session.Player.KeybindingManager.Update(biInputKeySet);
            else
                session.Account.KeybindingManager.Update(biInputKeySet);
        }
    }
}
