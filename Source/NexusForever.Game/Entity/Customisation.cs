using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Game.Entity
{
    public class Customisation : ICustomisation
    {
        [Flags]
        public enum CustomisationSaveMask
        {
            None   = 0x0000,
            Create = 0x0001,
            Modify = 0x0002,
            Delete = 0x0004
        }

        public ulong CharacterId { get; }
        public uint Label { get; }

        public uint Value
        {
            get => value;
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    saveMask |= CustomisationSaveMask.Modify;
                }
            }
        }
        private uint value;

        public bool PendingDelete => (saveMask & CustomisationSaveMask.Delete) != 0;

        private CustomisationSaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="ICustomisation"/> from database model.
        /// </summary>
        public Customisation(CharacterCustomisationModel model)
        {
            CharacterId = model.Id;
            Label       = model.Label;
            value       = model.Value;

            saveMask    = CustomisationSaveMask.None;
        }

        /// <summary>
        /// Create a new <see cref="ICustomisation"/> from supplied data.
        /// </summary>
        public Customisation(ulong characterId, uint label, uint value)
        {
            CharacterId = characterId;
            Label       = label;
            this.value  = value;

            saveMask    = CustomisationSaveMask.Create;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask == CustomisationSaveMask.None)
                return;

            var model = new CharacterCustomisationModel
            {
                Id    = CharacterId,
                Label = Label
            };

            EntityEntry<CharacterCustomisationModel> entity = context.Attach(model);

            if ((saveMask & CustomisationSaveMask.Create) != 0)
            {
                model.Value = Value;

                context.Add(model);
            }
            else if ((saveMask & CustomisationSaveMask.Delete) != 0)
            {
                context.Entry(model).State = EntityState.Deleted;
            }
            else if ((saveMask & CustomisationSaveMask.Modify) != 0)
            {
                model.Value = Value;
                entity.Property(e => e.Value).IsModified = true;
            }

            saveMask = CustomisationSaveMask.None;
        }

        public void Delete()
        {
            if ((saveMask & CustomisationSaveMask.Create) != 0)
                saveMask = CustomisationSaveMask.None;
            else
                saveMask = CustomisationSaveMask.Delete;
        }
    }
}
