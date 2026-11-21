using NexusForever.Game.Abstract.Spell;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Spell;

namespace NexusForever.WorldServer.Network.Message.Handler.Spell
{
    public class ClientCancelEffectHandler : IMessageHandler<IWorldSession, ClientCancelEffect>
    {
        public void HandleMessage(IWorldSession session, ClientCancelEffect cancelSpell)
        {
            ISpell spell = session.Player.GetSpell(cancelSpell.ServerUniqueId);
            if (spell == null)
                return;

            spell.Finish();
        }
    }
}
