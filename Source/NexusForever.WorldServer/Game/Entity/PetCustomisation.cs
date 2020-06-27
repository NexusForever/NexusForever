using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Game.Entity
{
    public class PetCustomisation : ISaveCharacter, IEnumerable<PetFlairEntry>
    {
        public ulong Owner { get; }
        public PetType Type { get; }
        public uint ObjectId { get; }

        public string Name
        {
            get => name;
            set
            {
                name = value;
                saveMask |= PetCustomisationSaveMask.Name;
            }
        }

        private string name;

        private readonly PetFlairEntry[] flairs = new PetFlairEntry[PetCustomisationManager.MaxCustomisationFlairs];

        private PetCustomisationSaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="PetCustomisation"/> from existing <see cref="CharacterPetCustomisationModel"/> database model.
        /// </summary>
        public PetCustomisation(CharacterPetCustomisationModel model)
        {
            Owner    = model.Id;
            Type     = (PetType)model.Type;
            ObjectId = model.ObjectId;
            Name     = model.Name;

            for (int i = 0; i < PetCustomisationManager.MaxCustomisationFlairs; i++)
            {
                uint flairId = (uint)(model.FlairIdMask >> i * 16) & 0xFFFF;
                flairs[i] = GameTableManager.Instance.PetFlair.GetEntry(flairId);
            }
        }

        /// <summary>
        /// Create a new <see cref="PetCustomisation"/> from supplied <see cref="PetType"/> and object id.
        /// </summary>
        public PetCustomisation(ulong owner, PetType type, uint objectId)
        {
            Owner    = owner;
            Type     = type;
            ObjectId = objectId;

            saveMask = PetCustomisationSaveMask.Create;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask == PetCustomisationSaveMask.None)
                return;

            if ((saveMask & PetCustomisationSaveMask.Create) != 0)
            {
                // pet customisation doesn't exist in database, all infomation must be saved
                var model = new CharacterPetCustomisationModel
                {
                    Id          = Owner,
                    Type        = (byte)Type,
                    ObjectId    = ObjectId,
                    Name        = Name,
                    FlairIdMask = GenerateFlairMask()
            };

                context.Add(model);
            }
            else
            {
                // pet customisation already exists in database, save only data that has been modified
                var model = new CharacterPetCustomisationModel
                {
                    Id       = Owner,
                    Type     = (byte)Type,
                    ObjectId = ObjectId
                };

                EntityEntry<CharacterPetCustomisationModel> entity = context.Attach(model);
                if ((saveMask & PetCustomisationSaveMask.Name) != 0)
                {
                    model.Name = Name;
                    entity.Property(p => p.Name).IsModified = true;
                }
                if ((saveMask & PetCustomisationSaveMask.Flairs) != 0)
                {
                    model.FlairIdMask = GenerateFlairMask();
                    entity.Property(p => p.FlairIdMask).IsModified = true;
                }
            }

            saveMask = PetCustomisationSaveMask.None;
        }

        /// <summary>
        /// Pack all flairs into a single <see cref="ulong"/>.
        /// </summary>
        private ulong GenerateFlairMask()
        {
            // pack each of the flair ids into 16 bits
            return ((flairs[3]?.Id ?? 0ul) << 48) | ((flairs[2]?.Id ?? 0ul) << 32) | ((flairs[1]?.Id ?? 0ul) << 16) | flairs[0]?.Id ?? 0ul;
        }

        /// <summary>
        /// Add or update flair from supplied <see cref="PetFlairEntry"/> at index.
        /// </summary>
        public void AddFlair(ushort index, PetFlairEntry entry)
        {
            flairs[index] = entry;
            saveMask |= PetCustomisationSaveMask.Flairs;
        }

        public IEnumerator<PetFlairEntry> GetEnumerator()
        {
            return flairs.Cast<PetFlairEntry>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
