using NexusForever.Game.Abstract.Spell;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Abilities;

namespace NexusForever.WorldServer.Network.Message.Handler.Spell
{
    public class ClientRespecAmpsHandler : IMessageHandler<IWorldSession, ClientRespecAmps>
    {
        public void HandleMessage(IWorldSession session, ClientRespecAmps requestAmpReset)
        {
            // TODO: check for client validity 
            // TODO: handle reset cost

            IActionSet actionSet = session.Player.SpellManager.GetActionSet(requestAmpReset.SpecIndex);
            actionSet.RemoveAmp(requestAmpReset.RespecType, requestAmpReset.Value);

            // a deselected AMP locks the SpellLevel rows it gated, in this spec only
            session.Player.SpellManager.RefreshGatedSpells();

            session.EnqueueMessageEncrypted(actionSet.BuildServerAmpList());
        }
    }
}
