using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Spell;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Pet
{
    public class ClientSummonVanityPetHandler : IMessageHandler<IWorldSession, ClientSummonVanityPet>
    {
        public void HandleMessage(IWorldSession session, ClientSummonVanityPet summonVanityPet)
        {
            ICharacterSpell spell = session.Player.SpellManager.GetSpell(summonVanityPet.Spell4BaseId);
            if (spell == null)
                throw new InvalidPacketValueException();

            byte tier = session.Player.SpellManager.GetSpellTier(spell.BaseInfo.Entry.Id);
            session.Player.CastSpell(new SpellParameters
            {
                SpellInfo = spell.BaseInfo.GetSpellInfo(tier)
            });
        }
    }
}
