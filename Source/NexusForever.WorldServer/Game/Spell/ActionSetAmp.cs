using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Game.Spell
{
    public class ActionSetAmp : ISaveCharacter
    {
        public EldanAugmentationEntry Entry { get; set; }

        /// <summary>
        /// Returns if <see cref="ActionSetAmp"/> is enqueued to be saved to the database.
        /// </summary>
        public bool PendingCreate => (saveMask & AmpSaveMask.Create) != 0;

        /// <summary>
        /// Returns if <see cref="ActionSetAmp"/> is enqueued to be deleted from the database.
        /// </summary>
        public bool PendingDelete => (saveMask & AmpSaveMask.Delete) != 0;

        private AmpSaveMask saveMask;
        private readonly ActionSet actionSet;

        /// <summary>
        /// Create a new <see cref="ActionSetAmp"/> from supplied <see cref="EldanAugmentationEntry"/>.
        /// </summary>
        public ActionSetAmp(ActionSet actionSet, EldanAugmentationEntry entry, bool isDirty)
        {
            Entry          = entry;
            this.actionSet = actionSet;

            if (isDirty)
                saveMask = AmpSaveMask.Create;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask == AmpSaveMask.None)
                return;

            var model = new CharacterActionSetAmpModel
            {
                Id        = actionSet.Owner,
                SpecIndex = actionSet.Index,
                AmpId     = (byte)Entry.Id
            };

            if ((saveMask & AmpSaveMask.Create) != 0)
                context.Add(model);
            else if ((saveMask & AmpSaveMask.Delete) != 0)
                context.Entry(model).State = EntityState.Deleted;

            saveMask = AmpSaveMask.None;
        }

        /// <summary>
        /// Enqueue or dequeue <see cref="ActionSetAmp"/> to be deleted from the database.
        /// </summary>
        public void EnqueueDelete(bool set)
        {
            if (set)
                saveMask |= AmpSaveMask.Delete;
            else
                saveMask &= ~AmpSaveMask.Delete;
        }
    }
}
