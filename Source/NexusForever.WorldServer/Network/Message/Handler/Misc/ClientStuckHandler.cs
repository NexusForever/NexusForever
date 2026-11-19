using NexusForever.Game.Spell;
using NexusForever.Game.Static.Support;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Support;

namespace NexusForever.WorldServer.Network.Message.Handler.Misc
{
    /// <summary>
    /// Sent from the stuck window ("Help, I'm Stuck!" or /stuck).
    /// </summary>
    public class ClientStuckHandler : IMessageHandler<IWorldSession, ClientStuck>
    {
        public void HandleMessage(IWorldSession session, ClientStuck stuck)
        {
            uint spell4Id = stuck.UnstickingType switch
            {
                UnstickType.RecallTransmat => 49881u,
                UnstickType.RecallHouse    => 49882u,
                _                          => 0u
            };

            // TODO: FreeSuicide (52577)
            if (spell4Id == 0u)
                return;

            session.Player.CastSpell(spell4Id, new SpellParameters
            {
                UserInitiatedSpellCast = true
            });
        }
    }
}
