using System.Collections.Generic;
using System.Linq;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Abilities;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.WorldServer.Network.Message.Handler.Spell
{
    public class ClientCommitAmpSpecHandler : IMessageHandler<IWorldSession, ClientCommitAmpSpec>
    {
        public void HandleMessage(IWorldSession session, ClientCommitAmpSpec clientCommitAmpSpec)
        {
            byte specIndex = session.Player.SpellManager.ActiveActionSet;

            IActionSet actionSet = session.Player.SpellManager.GetActionSet(specIndex);

            // the client sends the active spec's existing selection plus the pending additions, so only the
            // ids that are not selected yet are new.
            List<ushort> newAmps = clientCommitAmpSpec.Amps
                .Where(id => actionSet.GetAmp(id) == null)
                .Distinct()
                .ToList();

            if (newAmps.Count > 0)
            {
                LimitedActionSetResult result = session.Player.SpellManager.ValidateAmps(specIndex, newAmps);
                if (result != LimitedActionSetResult.Ok)
                {
                    // the client has no reply for a rejected commit other than the list it already holds,
                    // so send the unchanged selection back.
                    session.EnqueueMessageEncrypted(actionSet.BuildServerAmpList());
                    return;
                }

                foreach (ushort id in newAmps)
                    actionSet.AddAmp(id);
            }

            session.EnqueueMessageEncrypted(actionSet.BuildServerAmpList());
        }
    }
}
