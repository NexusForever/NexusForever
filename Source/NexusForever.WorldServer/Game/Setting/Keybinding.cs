using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.WorldServer.Game.Setting.Static;
using NetworkBinding = NexusForever.WorldServer.Network.Message.Model.Shared.Binding;

namespace NexusForever.WorldServer.Game.Setting
{
    public class Keybinding : ISaveCharacter, ISaveAuth
    {
        public ulong Owner { get; }
        public ushort InputActionId { get; set; }
        public uint DeviceEnum00 { get; set; }
        public uint DeviceEnum01 { get; set; }
        public uint DeviceEnum02 { get; set; }
        public uint Code00 { get; set; }
        public uint Code01 { get; set; }
        public uint Code02 { get; set; }
        public uint MetaKeys00 { get; set; }
        public uint MetaKeys01 { get; set; }
        public uint MetaKeys02 { get; set; }
        public uint EventTypeEnum00 { get; set; }
        public uint EventTypeEnum01 { get; set; }
        public uint EventTypeEnum02 { get; set; }

        /// <summary>
        /// Returns if <see cref="Keybinding"/> is enqueued to be deleted from the database.
        /// </summary>
        public bool PendingCreate => (saveMask & BindingSaveMask.Create) != 0;

        /// <summary>
        /// Returns if <see cref="Keybinding"/> is enqueued to be deleted from the database.
        /// </summary>
        public bool PendingDelete => (saveMask & BindingSaveMask.Delete) != 0;

        private BindingSaveMask saveMask;

        /// <summary>
        /// Create a new <see cref="Keybinding"/> from an existing database model.
        /// </summary>
        public Keybinding(ulong owner, AccountKeybindingModel model)
        {
            Owner           = owner;
            InputActionId   = model.InputActionId;
            DeviceEnum00    = model.DeviceEnum00;
            DeviceEnum01    = model.DeviceEnum01;
            DeviceEnum02    = model.DeviceEnum02;
            Code00          = model.Code00;
            Code01          = model.Code01;
            Code02          = model.Code02;
            MetaKeys00      = model.MetaKeys00;
            MetaKeys01      = model.MetaKeys01;
            MetaKeys02      = model.MetaKeys02;
            EventTypeEnum00 = model.EventTypeEnum00;
            EventTypeEnum01 = model.EventTypeEnum01;
            EventTypeEnum02 = model.EventTypeEnum02;
        }

        /// <summary>
        /// Create a new <see cref="Keybinding"/> from an existing database model.
        /// </summary>
        public Keybinding(ulong owner, CharacterKeybindingModel model)
        {
            Owner           = owner;
            InputActionId   = model.InputActionId;
            DeviceEnum00    = model.DeviceEnum00;
            DeviceEnum01    = model.DeviceEnum01;
            DeviceEnum02    = model.DeviceEnum02;
            Code00          = model.Code00;
            Code01          = model.Code01;
            Code02          = model.Code02;
            MetaKeys00      = model.MetaKeys00;
            MetaKeys01      = model.MetaKeys01;
            MetaKeys02      = model.MetaKeys02;
            EventTypeEnum00 = model.EventTypeEnum00;
            EventTypeEnum01 = model.EventTypeEnum01;
            EventTypeEnum02 = model.EventTypeEnum02;
        }

        /// <summary>
        /// Create a new <see cref="Keybinding"/> from a network model.
        /// </summary>
        public Keybinding(ulong owner, NetworkBinding networkBinding)
        {
            Owner           = owner;
            InputActionId   = networkBinding.InputActionId;
            DeviceEnum00    = networkBinding.DeviceEnum00;
            DeviceEnum01    = networkBinding.DeviceEnum01;
            DeviceEnum02    = networkBinding.DeviceEnum02;
            Code00          = networkBinding.Code00;
            Code01          = networkBinding.Code01;
            Code02          = networkBinding.Code02;
            MetaKeys00      = networkBinding.MetaKeys00;
            MetaKeys01      = networkBinding.MetaKeys01;
            MetaKeys02      = networkBinding.MetaKeys02;
            EventTypeEnum00 = networkBinding.EventTypeEnum00;
            EventTypeEnum01 = networkBinding.EventTypeEnum01;
            EventTypeEnum02 = networkBinding.EventTypeEnum02;
            saveMask        = BindingSaveMask.Create;
        }

        public void Save(CharacterContext context)
        {
            if ((saveMask & BindingSaveMask.Create) != 0)
            {
                var model = new CharacterKeybindingModel
                {
                    Id              = Owner,
                    InputActionId   = InputActionId,
                    DeviceEnum00    = DeviceEnum00,
                    DeviceEnum01    = DeviceEnum01,
                    DeviceEnum02    = DeviceEnum02,
                    Code00          = Code00,
                    Code01          = Code01,
                    Code02          = Code02,
                    MetaKeys00      = MetaKeys00,
                    MetaKeys01      = MetaKeys01,
                    MetaKeys02      = MetaKeys02,
                    EventTypeEnum00 = EventTypeEnum00,
                    EventTypeEnum01 = EventTypeEnum01,
                    EventTypeEnum02 = EventTypeEnum02

                };
                context.Add(model);
            }
            else 
            {
                var model = new CharacterKeybindingModel
                {
                    Id            = Owner,
                    InputActionId = InputActionId
                };

                if ((saveMask & BindingSaveMask.Delete) != 0)
                    context.Entry(model).State = EntityState.Deleted;
                else
                {
                    EntityEntry<CharacterKeybindingModel> entity = context.Attach(model);

                    if ((saveMask & BindingSaveMask.DeviceEnum00) != 0)
                    {
                        model.DeviceEnum00 = DeviceEnum00;
                        entity.Property(p => p.DeviceEnum00).IsModified = true;
                    }

                    if ((saveMask & BindingSaveMask.DeviceEnum01) != 0)
                    {
                        model.DeviceEnum01 = DeviceEnum01;
                        entity.Property(p => p.DeviceEnum01).IsModified = true;
                    }

                    if ((saveMask & BindingSaveMask.DeviceEnum02) != 0)
                    {
                        model.DeviceEnum02 = DeviceEnum02;
                        entity.Property(p => p.DeviceEnum02).IsModified = true;
                    }

                    if ((saveMask & BindingSaveMask.Code00) != 0)
                    {
                        model.Code00 = Code00;
                        entity.Property(p => p.Code00).IsModified = true;
                    }

                    if ((saveMask & BindingSaveMask.Code01) != 0)
                    {
                        model.Code01 = Code01;
                        entity.Property(p => p.Code01).IsModified = true;
                    }

                    if ((saveMask & BindingSaveMask.Code02) != 0)
                    {
                        model.Code02 = Code02;
                        entity.Property(p => p.Code02).IsModified = true;
                    }

                    if ((saveMask & BindingSaveMask.MetaKeys00) != 0)
                    {
                        model.MetaKeys00 = MetaKeys00;
                        entity.Property(p => p.MetaKeys00).IsModified = true;
                    }

                    if ((saveMask & BindingSaveMask.MetaKeys01) != 0)
                    {
                        model.MetaKeys01 = MetaKeys01;
                        entity.Property(p => p.MetaKeys01).IsModified = true;
                    }

                    if ((saveMask & BindingSaveMask.MetaKeys02) != 0)
                    {
                        model.MetaKeys02 = MetaKeys02;
                        entity.Property(p => p.MetaKeys02).IsModified = true;
                    }

                    if ((saveMask & BindingSaveMask.EventTypeEnum00) != 0)
                    {
                        model.EventTypeEnum00 = EventTypeEnum00;
                        entity.Property(p => p.EventTypeEnum00).IsModified = true;
                    }

                    if ((saveMask & BindingSaveMask.EventTypeEnum01) != 0)
                    {
                        model.EventTypeEnum01 = EventTypeEnum01;
                        entity.Property(p => p.EventTypeEnum01).IsModified = true;
                    }

                    if ((saveMask & BindingSaveMask.EventTypeEnum02) != 0)
                    {
                        model.EventTypeEnum02 = EventTypeEnum02;
                        entity.Property(p => p.EventTypeEnum02).IsModified = true;
                    }

                    saveMask = BindingSaveMask.None;
                }
            }
        }

        public void Save(AuthContext context)
        {
            if ((saveMask & BindingSaveMask.Create) != 0)
            {
                var model = new AccountKeybindingModel
                {
                    Id              = (uint)Owner,
                    InputActionId   = InputActionId,
                    DeviceEnum00    = DeviceEnum00,
                    DeviceEnum01    = DeviceEnum01,
                    DeviceEnum02    = DeviceEnum02,
                    Code00          = Code00,
                    Code01          = Code01,
                    Code02          = Code02,
                    MetaKeys00      = MetaKeys00,
                    MetaKeys01      = MetaKeys01,
                    MetaKeys02      = MetaKeys02,
                    EventTypeEnum00 = EventTypeEnum00,
                    EventTypeEnum01 = EventTypeEnum01,
                    EventTypeEnum02 = EventTypeEnum02

                };
                context.Add(model);
            }
            else 
            {
                var model = new AccountKeybindingModel
                {
                    Id              = (uint)Owner,
                    InputActionId   = InputActionId
                };

                if ((saveMask & BindingSaveMask.Delete) != 0)
                    context.Entry(model).State = EntityState.Deleted;
                else
                {
                    EntityEntry<AccountKeybindingModel> entity = context.Attach(model);

                    if ((saveMask & BindingSaveMask.DeviceEnum00) != 0)
                    {
                        model.DeviceEnum00 = DeviceEnum00;
                        entity.Property(p => p.DeviceEnum00).IsModified = true;
                    }

                    if ((saveMask & BindingSaveMask.DeviceEnum01) != 0)
                    {
                        model.DeviceEnum01 = DeviceEnum01;
                        entity.Property(p => p.DeviceEnum01).IsModified = true;
                    }

                    if ((saveMask & BindingSaveMask.DeviceEnum02) != 0)
                    {
                        model.DeviceEnum02 = DeviceEnum02;
                        entity.Property(p => p.DeviceEnum02).IsModified = true;
                    }

                    if ((saveMask & BindingSaveMask.Code00) != 0)
                    {
                        model.Code00 = Code00;
                        entity.Property(p => p.Code00).IsModified = true;
                    }

                    if ((saveMask & BindingSaveMask.Code01) != 0)
                    {
                        model.Code01 = Code01;
                        entity.Property(p => p.Code01).IsModified = true;
                    }

                    if ((saveMask & BindingSaveMask.Code02) != 0)
                    {
                        model.Code02 = Code02;
                        entity.Property(p => p.Code02).IsModified = true;
                    }

                    if ((saveMask & BindingSaveMask.MetaKeys00) != 0)
                    {
                        model.MetaKeys00 = MetaKeys00;
                        entity.Property(p => p.MetaKeys00).IsModified = true;
                    }

                    if ((saveMask & BindingSaveMask.MetaKeys01) != 0)
                    {
                        model.MetaKeys01 = MetaKeys01;
                        entity.Property(p => p.MetaKeys01).IsModified = true;
                    }

                    if ((saveMask & BindingSaveMask.MetaKeys02) != 0)
                    {
                        model.MetaKeys02 = MetaKeys02;
                        entity.Property(p => p.MetaKeys02).IsModified = true;
                    }

                    if ((saveMask & BindingSaveMask.EventTypeEnum00) != 0)
                    {
                        model.EventTypeEnum00 = EventTypeEnum00;
                        entity.Property(p => p.EventTypeEnum00).IsModified = true;
                    }

                    if ((saveMask & BindingSaveMask.EventTypeEnum01) != 0)
                    {
                        model.EventTypeEnum01 = EventTypeEnum01;
                        entity.Property(p => p.EventTypeEnum01).IsModified = true;
                    }

                    if ((saveMask & BindingSaveMask.EventTypeEnum02) != 0)
                    {
                        model.EventTypeEnum02 = EventTypeEnum02;
                        entity.Property(p => p.EventTypeEnum02).IsModified = true;
                    }

                    saveMask = BindingSaveMask.None;
                }
            }
        }

        /// <summary>
        /// Enqueue or dequeue <see cref="Keybinding"/> to be deleted from the database.
        /// </summary>
        public void EnqueueDelete(bool set)
        {
            if (set)
                saveMask |= BindingSaveMask.Delete;
            else
                saveMask &= ~BindingSaveMask.Delete;
        }

        /// <summary>
        /// Update <see cref="Keybinding"/> with information from supplied <see cref="NetworkBinding"/> from client.
        /// </summary>
        public void Update(NetworkBinding networkBinding)
        {
            if (DeviceEnum00 != networkBinding.DeviceEnum00)
            {
                DeviceEnum00 = networkBinding.DeviceEnum00;
                saveMask |= BindingSaveMask.DeviceEnum00;
            }

            if (DeviceEnum01 != networkBinding.DeviceEnum01)
            {
                DeviceEnum01 = networkBinding.DeviceEnum01;
                saveMask |= BindingSaveMask.DeviceEnum01;
            }

            if (DeviceEnum02 != networkBinding.DeviceEnum02)
            {
                DeviceEnum02 = networkBinding.DeviceEnum02;
                saveMask |= BindingSaveMask.DeviceEnum02;
            }

            if (Code00 != networkBinding.Code00)
            {
                Code00 = networkBinding.Code00;
                saveMask |= BindingSaveMask.Code00;
            }

            if (Code01 != networkBinding.Code01)
            {
                Code01 = networkBinding.Code01;
                saveMask |= BindingSaveMask.Code01;
            }

            if (Code02 != networkBinding.Code02)
            {
                Code02 = networkBinding.Code02;
                saveMask |= BindingSaveMask.Code02;
            }

            if (MetaKeys00 != networkBinding.MetaKeys00)
            {
                MetaKeys00 = networkBinding.MetaKeys00;
                saveMask |= BindingSaveMask.MetaKeys00;
            }

            if (MetaKeys01 != networkBinding.MetaKeys01)
            {
                MetaKeys01 = networkBinding.MetaKeys01;
                saveMask |= BindingSaveMask.MetaKeys01;
            }

            if (MetaKeys02 != networkBinding.MetaKeys02)
            {
                MetaKeys02 = networkBinding.MetaKeys02;
                saveMask |= BindingSaveMask.MetaKeys02;
            }

            if (EventTypeEnum00 != networkBinding.EventTypeEnum00)
            {
                EventTypeEnum00 = networkBinding.EventTypeEnum00;
                saveMask |= BindingSaveMask.EventTypeEnum00;
            }

            if (EventTypeEnum01 != networkBinding.EventTypeEnum01)
            {
                EventTypeEnum01 = networkBinding.EventTypeEnum01;
                saveMask |= BindingSaveMask.EventTypeEnum01;
            }

            if (EventTypeEnum02 != networkBinding.EventTypeEnum02)
            {
                EventTypeEnum02 = networkBinding.EventTypeEnum02;
                saveMask |= BindingSaveMask.EventTypeEnum02;
            }
        }
    }
}
