using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Spell
{
    public class ClientInnateChangeHandler : IMessageHandler<IWorldSession, ClientInnateChange>
    {
        public void HandleMessage(IWorldSession session, ClientInnateChange innateChange)
        {
            session.Player.SpellManager.SetInnate(innateChange.InnateIndex);
        }
    }
}
