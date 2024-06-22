using NexusForever.Game.Abstract.Spell;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Spell
{
    public class ClientRequestAmpResetHandler : IMessageHandler<IWorldSession, ClientRequestAmpReset>
    {
        public void HandleMessage(IWorldSession session, ClientRequestAmpReset requestAmpReset)
        {
            // TODO: check for client validity 
            // TODO: handle reset cost

            IActionSet actionSet = session.Player.SpellManager.GetActionSet(requestAmpReset.ActionSetIndex);
            actionSet.RemoveAmp(requestAmpReset.RespecType, requestAmpReset.Value);
            session.EnqueueMessageEncrypted(actionSet.BuildServerAmpList());
        }
    }
}
