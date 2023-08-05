using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Spell;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class PetHandler
    {
        [MessageHandler(GameMessageOpcode.ClientSummonVanityPet)]
        public static void HandleClientSummonVanityPet(IWorldSession session, ClientSummonVanityPet summonVanityPet)
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

        [MessageHandler(GameMessageOpcode.ClientPetCustomisation)]
        public static void HandleClientPetCustomisation(IWorldSession session, ClientPetCustomisation petcustomisation)
        {
            session.Player.PetCustomisationManager.AddCustomisation(petcustomisation.PetType,
                petcustomisation.PetObjectId,
                petcustomisation.FlairSlotIndex,
                petcustomisation.FlairId);
        }

        [MessageHandler(GameMessageOpcode.ClientPetRename)]
        public static void HandlePetRename(IWorldSession session, ClientPetRename petRename)
        {
            session.Player.PetCustomisationManager.RenamePet(petRename.PetType,
               petRename.PetObjectId,
               petRename.Name);
        }
    }
}
