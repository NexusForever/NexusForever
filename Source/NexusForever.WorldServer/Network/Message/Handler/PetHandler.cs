using NexusForever.Game.Network;
using NexusForever.Game.Spell;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class PetHandler
    {
        [MessageHandler(GameMessageOpcode.ClientSummonVanityPet)]
        public static void HandleClientSummonVanityPet(WorldSession session, ClientSummonVanityPet summonVanityPet)
        {
            CharacterSpell spell = session.Player.SpellManager.GetSpell(summonVanityPet.Spell4BaseId);
            if (spell == null)
                throw new InvalidPacketValueException();

            byte tier = session.Player.SpellManager.GetSpellTier(spell.BaseInfo.Entry.Id);
            session.Player.CastSpell(new SpellParameters
            {
                SpellInfo = spell.BaseInfo.GetSpellInfo(tier)
            });
        }

        [MessageHandler(GameMessageOpcode.ClientPetCustomisation)]
        public static void HandleClientPetCustomisation(WorldSession session, ClientPetCustomisation petcustomisation)
        {
            session.Player.PetCustomisationManager.AddCustomisation(petcustomisation.PetType,
                petcustomisation.PetObjectId,
                petcustomisation.FlairSlotIndex,
                petcustomisation.FlairId);
        }
    }
}
