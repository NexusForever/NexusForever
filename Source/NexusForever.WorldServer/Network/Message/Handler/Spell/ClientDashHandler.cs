using NexusForever.Game.Spell;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Spell
{
    public class ClientDashHandler : IMessageHandler<IWorldSession, ClientDash>
    {
        public void HandleMessage(IWorldSession session, ClientDash clientDash)
        {
            if (!SpellLookupManager.Instance.TryGetDashSpell(clientDash.DashDirection, out uint dashSpell4Id))
                return;

            session.Player.CastSpell(dashSpell4Id, new SpellParameters
            {
                UserInitiatedSpellCast = false
            });
        }
    }
}
