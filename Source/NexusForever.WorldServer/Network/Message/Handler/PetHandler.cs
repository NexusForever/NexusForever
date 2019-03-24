using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Spell;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class PetHandler
    {
        [MessageHandler(GameMessageOpcode.ClientSummonVanityPet)]
        public static void HandleClientSummonVanityPet(WorldSession session, ClientSummonVanityPet summonVanityPet)
        {
            UnlockedSpell spell = session.Player.SpellManager.GetSpell(summonVanityPet.Spell4BaseId);
            if (spell == null)
                throw new InvalidPacketValueException();

            byte tier = session.Player.SpellManager.GetSpellTier(spell.Info.Entry.Id);
            session.Player.CastSpell(new SpellParameters
            {
                SpellInfo = spell.Info.GetSpellInfo(tier)
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
