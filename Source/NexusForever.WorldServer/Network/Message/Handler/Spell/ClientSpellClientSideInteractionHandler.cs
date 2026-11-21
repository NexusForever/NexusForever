using System;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Spell.Type;
using NexusForever.Game.Static.Spell;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Spell;

namespace NexusForever.WorldServer.Network.Message.Handler.Spell
{
    public class ClientSpellClientSideInteractionHandler : IMessageHandler<IWorldSession, ClientSpellClientSideInteraction>
    {
        public void HandleMessage(IWorldSession session, ClientSpellClientSideInteraction spellInteractionResult)
        {
            ISpell spell = session.Player.GetSpell(spellInteractionResult.ServerUniqueId);
            if (spell == null)
                throw new ArgumentNullException($"Spell cast {spellInteractionResult.ServerUniqueId} not found.");

            if (spell is not SpellClientSideInteraction spellCSI)
                throw new ArgumentNullException($"Spell missing a ClientSideInteraction.");

            switch (spellInteractionResult.CSIResponse)
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
