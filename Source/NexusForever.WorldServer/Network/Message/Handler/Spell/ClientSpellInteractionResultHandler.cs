using System;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Spell;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.WorldServer.Network.Message.Handler.Spell
{
    public class ClientSpellInteractionResultHandler : IMessageHandler<IWorldSession, ClientSpellInteractionResult>
    {
        public void HandleMessage(IWorldSession session, ClientSpellInteractionResult spellInteractionResult)
        {
            if (!(session.Player.HasSpell(x => x.CastingId == spellInteractionResult.CastingId, out ISpell spell)))
                throw new ArgumentNullException($"Spell cast {spellInteractionResult.CastingId} not found.");

            if (spell is not SpellClientSideInteraction spellCSI)
                throw new ArgumentNullException($"Spell missing a ClientSideInteraction.");

            switch (spellInteractionResult.Result)
            {
                case 0:
                    spellCSI.FailClientInteraction();
                    break;
                case 1:
                    spellCSI.SucceedClientInteraction();
                    break;
                case 2:
                    spellCSI.CancelCast(CastResult.ClientSideInteractionFail);
                    break;
            }
        }
    }
}
