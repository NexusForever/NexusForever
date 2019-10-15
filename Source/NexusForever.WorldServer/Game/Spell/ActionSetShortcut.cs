using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.WorldServer.Game.Spell.Static;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Game.Spell
{
    public class ActionSetShortcut : ISaveCharacter
    {
        public UILocation Location { get; }

        public ShortcutType ShortcutType
        {
            get => shortcutType;
            set
            {
                if (value != shortcutType)
                    saveMask |= ShortcutSaveMask.ShortcutType;
                shortcutType = value;
            }
        }

        private ShortcutType shortcutType;

        public uint ObjectId
        {
            get => objectId;
            set
            {
                if (value != objectId)
                    saveMask |= ShortcutSaveMask.ObjectId;
                objectId = value;
            }
        }

        private uint objectId;

        public byte Tier
        {
            get => tier;
            set
            {
                if (value != tier)
                    saveMask |= ShortcutSaveMask.Tier;
                tier = value;
            }
        }

        private byte tier;

        /// <summary>
        /// Returns if <see cref="ActionSetShortcut"/> is enqueued to be saved to the database.
        /// </summary>
        public bool PendingCreate => (saveMask & ShortcutSaveMask.Create) != 0;

        /// <summary>
        /// Returns if <see cref="ActionSetShortcut"/> is enqueued to be deleted from the database.
        /// </summary>
        public bool PendingDelete => (saveMask & ShortcutSaveMask.Delete) != 0;

        private ShortcutSaveMask saveMask;
        private readonly ActionSet actionSet;

        /// <summary>
        /// Create a new <see cref="ActionSetShortcut"/> from an existing database model.
        /// </summary>
        public ActionSetShortcut(ActionSet actionSet, CharacterActionSetShortcutModel model)
        {
            this.actionSet = actionSet;
            Location       = (UILocation)model.Location;
            ShortcutType   = (ShortcutType)model.ShortcutType;
            objectId       = model.ObjectId;
            tier           = model.Tier;
        }

        /// <summary>
        /// Create a new <see cref="ActionSetShortcut"/>.
        /// </summary>
        public ActionSetShortcut(ActionSet actionSet, UILocation location, ShortcutType shortcutType, uint objectId, byte tier)
        {
            this.actionSet = actionSet;
            Location       = location;
            ShortcutType   = shortcutType;
            ObjectId       = objectId;
            Tier           = tier;

            saveMask       = ShortcutSaveMask.Create;
        }

        public void Save(CharacterContext context)
        {
            if (saveMask == ShortcutSaveMask.None)
                return;

            if ((saveMask & ShortcutSaveMask.Create) != 0)
            {
                var model = new CharacterActionSetShortcutModel
                {
                    Id           = actionSet.Owner,
                    SpecIndex    = actionSet.Index,
                    Location     = (ushort)Location,
                    ShortcutType = (byte)ShortcutType,
                    ObjectId     = ObjectId,
                    Tier         = tier
                };

                context.Add(model);
            }
            else
            {
                var model = new CharacterActionSetShortcutModel
                {
                    Id        = actionSet.Owner,
                    SpecIndex = actionSet.Index,
                    Location  = (ushort)Location
                };

                if ((saveMask & ShortcutSaveMask.Delete) != 0)
                    context.Entry(model).State = EntityState.Deleted;
                else
                {
                    EntityEntry<CharacterActionSetShortcutModel> entity = context.Attach(model);
                    if ((saveMask & ShortcutSaveMask.ShortcutType) != 0)
                    {
                        model.ShortcutType = (byte)ShortcutType;
                        entity.Property(p => p.ShortcutType).IsModified = true;
                    }

                    if ((saveMask & ShortcutSaveMask.ObjectId) != 0)
                    {
                        model.ObjectId = ObjectId;
                        entity.Property(p => p.ObjectId).IsModified = true;
                    }

                    if ((saveMask & ShortcutSaveMask.Tier) != 0)
                    {
                        model.Tier = Tier;
                        entity.Property(p => p.Tier).IsModified = true;
                    }
                }
            }

            saveMask = ShortcutSaveMask.None;
        }

        /// <summary>
        /// Enqueue or dequeue <see cref="ActionSetShortcut"/> to be deleted from the database.
        /// </summary>
        public void EnqueueDelete(bool set)
        {
            if (set)
                saveMask |= ShortcutSaveMask.Delete;
            else
                saveMask &= ~ShortcutSaveMask.Delete;
        }
    }
}
