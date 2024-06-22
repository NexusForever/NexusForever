using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Spell
{
    public class ClientSpellStopCastHandler : IMessageHandler<IWorldSession, ClientSpellStopCast>
    {
        public void HandleMessage(IWorldSession session, ClientSpellStopCast spellStopCast)
        {
            // TODO: handle CastResult, client only sends SpellCancelled and SpellInterrupted
            session.Player.CancelSpellCast(spellStopCast.CastingId);
        }
    }
}
