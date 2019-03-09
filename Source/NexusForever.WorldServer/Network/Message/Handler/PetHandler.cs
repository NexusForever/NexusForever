using System;
using System.Collections.Generic;
using System.Linq;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Spell;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class PetHandler
    {
        [MessageHandler(GameMessageOpcode.ClientSummonVanityPet)]
        public static void HandleClientSummonVanityPet(WorldSession session, ClientSummonVanityPet castSpell)
        {
            UnlockedSpell spell = session.Player.SpellManager.GetSpell(castSpell.Spell4BaseId);
            if (spell == null)
                throw new InvalidPacketValueException();

            session.Player.CastSpell(new SpellParameters
            {
                SpellInfo = spell.Info.GetSpellInfo(spell.Tier)
            });
        }

        [MessageHandler(GameMessageOpcode.ClientPetCustomization)]
        public static void HandleClientPetCustomization(WorldSession session, ClientPetCustomization petCust)
        {
            // TODO: more sanity checks... e.g. if flair is unlocked

            var petCustomization = session.Player.PetCustomizations.FirstOrDefault(p => p.Spell4Id == petCust.Spell4Id);
            if (petCustomization != null && (petCust.FlairSlotIndex < 0 || petCust.FlairSlotIndex >= PetCustomization.MaxSlots))
                throw new InvalidPacketValueException();

            if (petCustomization == null)
            {
                petCustomization = new PetCustomization
                {
                    PetType = petCust.PetType,
                    Spell4Id = petCust.Spell4Id,
                };

                petCustomization.PetFlairIds[petCust.FlairSlotIndex] = petCust.PetFlairId;
                session.Player.PetCustomizations.Add(petCustomization);
            }
            else
            {
                petCustomization.PetType = petCust.PetType;
                petCustomization.PetFlairIds[petCust.FlairSlotIndex] = petCust.PetFlairId;
            }

            session.EnqueueMessageEncrypted(new ServerPetCustomization
            {
                petCustomization = petCustomization
            });
        }
    }
}