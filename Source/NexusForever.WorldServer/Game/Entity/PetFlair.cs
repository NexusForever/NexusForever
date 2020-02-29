using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;

namespace NexusForever.WorldServer.Game.Entity
{
    public class PetFlair : ISaveCharacter
    {
        public ulong Owner { get; }
        public PetFlairEntry Entry { get; }

        private bool isDirty;

        /// <summary>
        /// Create a new <see cref="PetFlair"/> from existing <see cref="CharacterPetFlairModel"/> database model.
        /// </summary>
        public PetFlair(CharacterPetFlairModel model)
        {
            Owner = model.Id;
            Entry = GameTableManager.Instance.PetFlair.GetEntry(model.PetFlairId);
        }

        /// <summary>
        /// Create a new <see cref="PetFlair"/> from supplied <see cref="PetFlairEntry"/>.
        /// </summary>
        public PetFlair(ulong owner, PetFlairEntry entry)
        {
            Entry   = entry;
            Owner   = owner;

            isDirty = true;
        }

        public void Save(CharacterContext context)
        {
            if (!isDirty)
                return;

            var model = new CharacterPetFlairModel
            {
                Id         = Owner,
                PetFlairId = Entry.Id
            };

            context.Add(model);
            isDirty = false;
        }
    }
}
