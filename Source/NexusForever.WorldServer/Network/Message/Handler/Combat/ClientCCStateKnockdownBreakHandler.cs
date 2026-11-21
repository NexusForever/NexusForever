using System;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Abstract.Spell.Target;
using NexusForever.Game.Static.Combat.CrowdControl;
using NexusForever.Game.Static.Entity.Movement;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Combat.CrowdControl;

namespace NexusForever.WorldServer.Network.Message.Handler.Combat
{
    public class ClientCCStateKnockdownBreakHandler : IMessageHandler<IWorldSession, ClientCCStateKnockdownBreak>
    {
        private readonly ISpellParameters spellParameters;

        public ClientCCStateKnockdownBreakHandler(
            ISpellParameters spellParameters)
        {
            this.spellParameters = spellParameters;
        }

        public void HandleMessage(IWorldSession session, ClientCCStateKnockdownBreak packet)
        {
            ISpellTargetEffectInfo info = session.Player.CrowdControlManager.GetCCEffect(CCState.Knockdown);
            if (info == null)
                return;

            // 28770 - Dash Left
            // 28771 - Dash Right
            // 28772 - Dash Forward (Knockdown) - Tier 1
            // 28773 - Dash Backward
            uint spell4Id = packet.Direction switch
            {
                DashDirection.Left     => 28770,
                DashDirection.Right    => 28771,
                DashDirection.Forward  => 28772,
                DashDirection.Back     => 28773,
                _                      => throw new NotImplementedException()
            };

            spellParameters.UserInitiatedSpellCast = false;
            session.Player.CastSpell(spell4Id, spellParameters);
        }
    }
}
