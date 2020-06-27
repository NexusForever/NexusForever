using System;
using System.Collections.Generic;
using System.Linq;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NetworkPetCustomisation = NexusForever.WorldServer.Network.Message.Model.Shared.PetCustomisation;

namespace NexusForever.WorldServer.Game.Entity
{
    public class PetCustomisationManager : ISaveCharacter
    {
        public const byte MaxCustomisationFlairs = 4;

        private static ulong PetCustomisationHash(PetType type, uint objectId)
        {
            return ((ulong)objectId << 32) | (byte)type;
        }

        private readonly Player player;
        private readonly Dictionary<uint/*petFlairId*/, PetFlair> petFlairs = new Dictionary<uint, PetFlair>(); 
        private readonly Dictionary<ulong/*hash*/, PetCustomisation> petCustomisations = new Dictionary<ulong, PetCustomisation>();

        /// <summary>
        /// Create a new <see cref="PetCustomisationManager"/> from existing <see cref="CharacterModel"/> database model.
        /// </summary>
        public PetCustomisationManager(Player owner, CharacterModel model)
        {
            player = owner;

            foreach (CharacterPetFlairModel flairModel in model.PetFlair)
            {
                var petFlair = new PetFlair(flairModel);
                petFlairs.Add(petFlair.Entry.Id, petFlair);
            }

            foreach (CharacterPetCustomisationModel customisationModel in model.PetCustomisation)
            {
                var customisation = new PetCustomisation(customisationModel);
                petCustomisations.Add(PetCustomisationHash(customisation.Type, customisation.ObjectId), customisation);
            }
        }

        public void Save(CharacterContext context)
        {
            foreach (PetFlair flair in petFlairs.Values)
                flair.Save(context);

            foreach (PetCustomisation customisation in petCustomisations.Values)
                customisation.Save(context);
        }

        /// <summary>
        /// Unlock pet flair with supplied id.
        /// </summary>
        public void UnlockFlair(ushort id)
        {
            PetFlairEntry entry = GameTableManager.Instance.PetFlair.GetEntry(id);
            if (entry == null)
                throw new ArgumentOutOfRangeException();

            if (petFlairs.ContainsKey(id))
                throw new ArgumentException();

            // TODO: check if prerequisites are met
            if (entry.PrerequisiteId > 0)
                return;

            petFlairs.Add(id, new PetFlair(player.CharacterId, entry));

            if (!player.IsLoading)
            {
                player.Session.EnqueueMessageEncrypted(new ServerUnlockPetFlair
                {
                    PetFlairId = id
                });
            }
        }

        /// <summary>
        /// Add or update equipped pet flair at supplied index for <see cref="PetType"/> and object id.
        /// </summary>
        public void AddCustomisation(PetType type, uint objectId, ushort index, ushort flairId)
        {
            if (index >= MaxCustomisationFlairs)
                throw new ArgumentOutOfRangeException();

            if (flairId != 0 && !petFlairs.ContainsKey(flairId))
                throw new ArgumentException();

            PetFlairEntry entry = GameTableManager.Instance.PetFlair.GetEntry(flairId);
            if (flairId != 0 && entry == null)
                throw new ArgumentException();

            ulong hash = PetCustomisationHash(type, objectId);
            if (!petCustomisations.TryGetValue(hash, out PetCustomisation customisation))
            {
                customisation = new PetCustomisation(player.CharacterId, type, objectId);
                petCustomisations.Add(hash, customisation);
            }

            customisation.AddFlair(index, entry);

            if (!player.IsLoading)
            {
                player.Session.EnqueueMessageEncrypted(new ServerPetCustomisation
                {
                    PetCustomisation = new NetworkPetCustomisation
                    {
                        PetType      = customisation.Type,
                        PetObjectId  = customisation.ObjectId,
                        PetName      = customisation.Name,
                        SlotFlairIds = customisation
                            .Select(f => f?.Id ?? 0)
                            .ToArray()
                    }
                });
            }
        }

        /// <summary>
        /// Return <see cref="PetCustomisation"/> for supplied <see cref="PetType"/> and object id.
        /// </summary>
        public PetCustomisation GetCustomisation(PetType type, uint objectId)
        {
            ulong hash = PetCustomisationHash(type, objectId);
            petCustomisations.TryGetValue(hash, out PetCustomisation customisation);
            return customisation;
        }

        public void SendInitialPackets()
        {
            var petCustomisationList = new ServerPetCustomisationList
            {
                PetCustomisations = petCustomisations.Values
                    .Select(c => new NetworkPetCustomisation
                    {
                        PetType      = c.Type,
                        PetObjectId  = c.ObjectId,
                        PetName      = c.Name,
                        SlotFlairIds = c
                            .Select(f => f?.Id ?? 0)
                            .ToArray()
                    })
                    .ToList()
            };

            foreach (uint flairBitIndex in petFlairs
                .SelectMany(f => f.Value.Entry.UnlockBitIndex))
                petCustomisationList.UnlockedFlair.SetBit(flairBitIndex, true);

            player.Session.EnqueueMessageEncrypted(petCustomisationList);
        }
    }
}
