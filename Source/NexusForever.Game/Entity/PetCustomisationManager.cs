using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.TextFilter;
using NexusForever.Game.Text.Filter;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Entity
{
    public class PetCustomisationManager : IPetCustomisationManager
    {
        public const byte MaxCustomisationFlairs = 4;

        private static ulong PetCustomisationHash(PetType type, uint objectId)
        {
            return (ulong)objectId << 32 | (byte)type;
        }

        private readonly IPlayer player;
        private readonly Dictionary<uint/*petFlairId*/, IPetFlair> petFlairs = new();
        private readonly Dictionary<ulong/*hash*/, IPetCustomisation> petCustomisations = new();

        /// <summary>
        /// Create a new <see cref="IPetCustomisationManager"/> from existing <see cref="CharacterModel"/> database model.
        /// </summary>
        public PetCustomisationManager(IPlayer owner, CharacterModel model)
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
            foreach (IPetFlair flair in petFlairs.Values)
                flair.Save(context);

            foreach (IPetCustomisation customisation in petCustomisations.Values)
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
        /// Renames the pet name for <see cref="PetType"/> and object id.
        /// </summary>
        public void RenamePet(PetType type, uint objectId, String name)
        {
            if (!TextFilterManager.Instance.IsTextValid(name, UserText.ScientistScanbotName))
                throw new InvalidPacketValueException();

            ulong hash = PetCustomisationHash(type, objectId);
            if (!petCustomisations.TryGetValue(hash, out IPetCustomisation customisation))
            {
                customisation = new PetCustomisation(player.CharacterId, type, objectId);
                petCustomisations.Add(hash, customisation);
            }

            customisation.Name = name;

            if (!player.IsLoading)
            {
                player.Session.EnqueueMessageEncrypted(new ServerPetCustomisation
                {
                    PetCustomisation = customisation.Build()
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
            if (!petCustomisations.TryGetValue(hash, out IPetCustomisation customisation))
            {
                customisation = new PetCustomisation(player.CharacterId, type, objectId);
                petCustomisations.Add(hash, customisation);
            }

            customisation.AddFlair(index, entry);

            if (!player.IsLoading)
            {
                player.Session.EnqueueMessageEncrypted(new ServerPetCustomisation
                {
                    PetCustomisation = customisation.Build()
                });
            }
        }

        /// <summary>
        /// Return <see cref="IPetCustomisation"/> for supplied <see cref="PetType"/> and object id.
        /// </summary>
        public IPetCustomisation GetCustomisation(PetType type, uint objectId)
        {
            ulong hash = PetCustomisationHash(type, objectId);
            petCustomisations.TryGetValue(hash, out IPetCustomisation customisation);
            return customisation;
        }

        public void SendInitialPackets()
        {
            var petCustomisationList = new ServerPetCustomisationList
            {
                PetCustomisations = petCustomisations.Values
                    .Select(c => c.Build())
                    .ToList()
            };

            foreach (uint flairBitIndex in petFlairs
                .SelectMany(f => f.Value.Entry.UnlockBitIndex))
                petCustomisationList.UnlockedFlair.SetBit(flairBitIndex, true);

            player.Session.EnqueueMessageEncrypted(petCustomisationList);
        }
    }
}
