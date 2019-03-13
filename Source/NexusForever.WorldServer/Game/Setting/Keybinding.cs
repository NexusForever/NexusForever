using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Shared.Database;
using NexusForever.Shared.Database.Auth.Model;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Setting.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NetworkBinding = NexusForever.WorldServer.Network.Message.Model.Shared.Binding;

namespace NexusForever.WorldServer.Game.Setting
{
    public class Keybinding : ISaveCharacter, ISaveAuth
    {
        public ulong Owner { get; }
        public InputSets InputSet { get; }

        public readonly Dictionary<uint, Binding> bindings = new Dictionary<uint, Binding>();

        private KeybindingSaveMask saveMask;

        public Keybinding(Character model)
        {
            Owner = model.Id;
            InputSet = InputSets.Character;

            foreach (CharacterKeybinding binding in model.CharacterKeybinding)
                bindings.Add(binding.InputActionId, new Binding(binding));
        }

        public Keybinding(Account model)
        {
            Owner = model.Id;
            InputSet = InputSets.Account;

            foreach (AccountKeybinding binding in model.AccountKeybinding)
                bindings.Add(binding.InputActionId, new Binding(binding));
        }

        public void Save(CharacterContext context)
        {
            if (saveMask == KeybindingSaveMask.None)
                return;

            foreach (Binding binding in bindings.Values.ToList())
            {
                if ((binding.saveMask & BindingSaveMask.Create) != 0)
                {
                    var model = new CharacterKeybinding
                    {
                        Id              = Owner,
                        InputActionId   = binding.InputActionId,
                        DeviceEnum00    = binding.DeviceEnum00,
                        DeviceEnum01    = binding.DeviceEnum01,
                        DeviceEnum02    = binding.DeviceEnum02,
                        Code00          = binding.Code00,
                        Code01          = binding.Code01,
                        Code02          = binding.Code02,
                        MetaKeys00      = binding.MetaKeys00,
                        MetaKeys01      = binding.MetaKeys01,
                        MetaKeys02      = binding.MetaKeys02,
                        EventTypeEnum00 = binding.EventTypeEnum00,
                        EventTypeEnum01 = binding.EventTypeEnum01,
                        EventTypeEnum02 = binding.EventTypeEnum02

                    };
                    context.Add(model);
                }
                else 
                {
                    var model = new CharacterKeybinding
                    {
                        Id              = Owner,
                        InputActionId   = binding.InputActionId
                    };

                    if((binding.saveMask & BindingSaveMask.Keep) == 0)
                    {
                        context.Entry(model).State = EntityState.Deleted;
                        bindings.Remove(binding.InputActionId);
                    }
                    else
                    {
                        EntityEntry<CharacterKeybinding> entity = context.Attach(model);

                        if((binding.saveMask & BindingSaveMask.DeviceEnum00) != 0)
                        {
                            model.DeviceEnum00 = binding.DeviceEnum00;
                            entity.Property(p => p.DeviceEnum00).IsModified = true;
                        }

                        if((binding.saveMask & BindingSaveMask.DeviceEnum01) != 0)
                        {
                            model.DeviceEnum01 = binding.DeviceEnum01;
                            entity.Property(p => p.DeviceEnum01).IsModified = true;
                        }

                        if((binding.saveMask & BindingSaveMask.DeviceEnum02) != 0)
                        {
                            model.DeviceEnum02 = binding.DeviceEnum02;
                            entity.Property(p => p.DeviceEnum02).IsModified = true;
                        }

                        if((binding.saveMask & BindingSaveMask.Code00) != 0)
                        {
                            model.Code00 = binding.Code00;
                            entity.Property(p => p.Code00).IsModified = true;
                        }

                        if((binding.saveMask & BindingSaveMask.Code01) != 0)
                        {
                            model.Code01 = binding.Code01;
                            entity.Property(p => p.Code01).IsModified = true;
                        }

                        if((binding.saveMask & BindingSaveMask.Code02) != 0)
                        {
                            model.Code02 = binding.Code02;
                            entity.Property(p => p.Code02).IsModified = true;
                        }

                        if((binding.saveMask & BindingSaveMask.MetaKeys00) != 0)
                        {
                            model.MetaKeys00 = binding.MetaKeys00;
                            entity.Property(p => p.MetaKeys00).IsModified = true;
                        }

                        if((binding.saveMask & BindingSaveMask.MetaKeys01) != 0)
                        {
                            model.MetaKeys01 = binding.MetaKeys01;
                            entity.Property(p => p.MetaKeys01).IsModified = true;
                        }

                        if((binding.saveMask & BindingSaveMask.MetaKeys02) != 0)
                        {
                            model.MetaKeys02 = binding.MetaKeys02;
                            entity.Property(p => p.MetaKeys02).IsModified = true;
                        }

                        if((binding.saveMask & BindingSaveMask.EventTypeEnum00) != 0)
                        {
                            model.EventTypeEnum00 = binding.EventTypeEnum00;
                            entity.Property(p => p.EventTypeEnum00).IsModified = true;
                        }

                        if((binding.saveMask & BindingSaveMask.EventTypeEnum01) != 0)
                        {
                            model.EventTypeEnum01 = binding.EventTypeEnum01;
                            entity.Property(p => p.EventTypeEnum01).IsModified = true;
                        }

                        if((binding.saveMask & BindingSaveMask.EventTypeEnum02) != 0)
                        {
                            model.EventTypeEnum02 = binding.EventTypeEnum02;
                            entity.Property(p => p.EventTypeEnum02).IsModified = true;
                        }

                        binding.saveMask = BindingSaveMask.None;
                    }
                }
            }

            saveMask = KeybindingSaveMask.None;
        }

        public void Save(AuthContext context)
        {
            if (saveMask == KeybindingSaveMask.None)
                return;

            foreach (Binding binding in bindings.Values.ToList())
            {
                if ((binding.saveMask & BindingSaveMask.Create) != 0)
                {
                    var model = new AccountKeybinding
                    {
                        Id              = (uint)Owner,
                        InputActionId   = binding.InputActionId,
                        DeviceEnum00    = binding.DeviceEnum00,
                        DeviceEnum01    = binding.DeviceEnum01,
                        DeviceEnum02    = binding.DeviceEnum02,
                        Code00          = binding.Code00,
                        Code01          = binding.Code01,
                        Code02          = binding.Code02,
                        MetaKeys00      = binding.MetaKeys00,
                        MetaKeys01      = binding.MetaKeys01,
                        MetaKeys02      = binding.MetaKeys02,
                        EventTypeEnum00 = binding.EventTypeEnum00,
                        EventTypeEnum01 = binding.EventTypeEnum01,
                        EventTypeEnum02 = binding.EventTypeEnum02

                    };
                    context.Add(model);
                }
                else 
                {
                    var model = new AccountKeybinding
                    {
                        Id              = (uint)Owner,
                        InputActionId   = binding.InputActionId
                    };

                    if((binding.saveMask & BindingSaveMask.Keep) == 0)
                    {
                        context.Entry(model).State = EntityState.Deleted;
                        bindings.Remove(binding.InputActionId);
                    }
                    else
                    {
                        EntityEntry<AccountKeybinding> entity = context.Attach(model);

                        if((binding.saveMask & BindingSaveMask.DeviceEnum00) != 0)
                        {
                            model.DeviceEnum00 = binding.DeviceEnum00;
                            entity.Property(p => p.DeviceEnum00).IsModified = true;
                        }

                        if((binding.saveMask & BindingSaveMask.DeviceEnum01) != 0)
                        {
                            model.DeviceEnum01 = binding.DeviceEnum01;
                            entity.Property(p => p.DeviceEnum01).IsModified = true;
                        }

                        if((binding.saveMask & BindingSaveMask.DeviceEnum02) != 0)
                        {
                            model.DeviceEnum02 = binding.DeviceEnum02;
                            entity.Property(p => p.DeviceEnum02).IsModified = true;
                        }

                        if((binding.saveMask & BindingSaveMask.Code00) != 0)
                        {
                            model.Code00 = binding.Code00;
                            entity.Property(p => p.Code00).IsModified = true;
                        }

                        if((binding.saveMask & BindingSaveMask.Code01) != 0)
                        {
                            model.Code01 = binding.Code01;
                            entity.Property(p => p.Code01).IsModified = true;
                        }

                        if((binding.saveMask & BindingSaveMask.Code02) != 0)
                        {
                            model.Code02 = binding.Code02;
                            entity.Property(p => p.Code02).IsModified = true;
                        }

                        if((binding.saveMask & BindingSaveMask.MetaKeys00) != 0)
                        {
                            model.MetaKeys00 = binding.MetaKeys00;
                            entity.Property(p => p.MetaKeys00).IsModified = true;
                        }

                        if((binding.saveMask & BindingSaveMask.MetaKeys01) != 0)
                        {
                            model.MetaKeys01 = binding.MetaKeys01;
                            entity.Property(p => p.MetaKeys01).IsModified = true;
                        }

                        if((binding.saveMask & BindingSaveMask.MetaKeys02) != 0)
                        {
                            model.MetaKeys02 = binding.MetaKeys02;
                            entity.Property(p => p.MetaKeys02).IsModified = true;
                        }

                        if((binding.saveMask & BindingSaveMask.EventTypeEnum00) != 0)
                        {
                            model.EventTypeEnum00 = binding.EventTypeEnum00;
                            entity.Property(p => p.EventTypeEnum00).IsModified = true;
                        }

                        if((binding.saveMask & BindingSaveMask.EventTypeEnum01) != 0)
                        {
                            model.EventTypeEnum01 = binding.EventTypeEnum01;
                            entity.Property(p => p.EventTypeEnum01).IsModified = true;
                        }

                        if((binding.saveMask & BindingSaveMask.EventTypeEnum02) != 0)
                        {
                            model.EventTypeEnum02 = binding.EventTypeEnum02;
                            entity.Property(p => p.EventTypeEnum02).IsModified = true;
                        }

                        binding.saveMask = BindingSaveMask.None;
                    }
                }
            }

            saveMask = KeybindingSaveMask.None;
        }

        public void Update(BiInputKeySet biInputKeySet)
        {
            if (bindings.Count + biInputKeySet.Bindings.Count == 0)
                return;

            saveMask = KeybindingSaveMask.Modify;

            foreach(NetworkBinding networkBinding in biInputKeySet.Bindings)
            {
                if (!bindings.TryGetValue(networkBinding.InputActionId, out Binding binding))
                    bindings.Add(networkBinding.InputActionId, new Binding(networkBinding));
                else
                {
                    binding.Keep();
                    if (binding.DeviceEnum00 != networkBinding.DeviceEnum00)
                    {
                        binding.DeviceEnum00 = networkBinding.DeviceEnum00;
                        binding.saveMask |= BindingSaveMask.DeviceEnum00;
                    }

                    if (binding.DeviceEnum01 != networkBinding.DeviceEnum01)
                    {
                        binding.DeviceEnum01 = networkBinding.DeviceEnum01;
                        binding.saveMask |= BindingSaveMask.DeviceEnum01;
                    }

                    if (binding.DeviceEnum02 != networkBinding.DeviceEnum02)
                    {
                        binding.DeviceEnum02 = networkBinding.DeviceEnum02;
                        binding.saveMask |= BindingSaveMask.DeviceEnum02;
                    }

                    if (binding.Code00 != networkBinding.Code00)
                    {
                        binding.Code00 = networkBinding.Code00;
                        binding.saveMask |= BindingSaveMask.Code00;
                    }

                    if (binding.Code01 != networkBinding.Code01)
                    {
                        binding.Code01 = networkBinding.Code01;
                        binding.saveMask |= BindingSaveMask.Code01;
                    }

                    if (binding.Code02 != networkBinding.Code02)
                    {
                        binding.Code02 = networkBinding.Code02;
                        binding.saveMask |= BindingSaveMask.Code02;
                    }

                    if (binding.MetaKeys00 != networkBinding.MetaKeys00)
                    {
                        binding.MetaKeys00 = networkBinding.MetaKeys00;
                        binding.saveMask |= BindingSaveMask.MetaKeys00;
                    }

                    if (binding.MetaKeys01 != networkBinding.MetaKeys01)
                    {
                        binding.MetaKeys01 = networkBinding.MetaKeys01;
                        binding.saveMask |= BindingSaveMask.MetaKeys01;
                    }

                    if (binding.MetaKeys02 != networkBinding.MetaKeys02)
                    {
                        binding.MetaKeys02 = networkBinding.MetaKeys02;
                        binding.saveMask |= BindingSaveMask.MetaKeys02;
                    }

                    if (binding.EventTypeEnum00 != networkBinding.EventTypeEnum00)
                    {
                        binding.EventTypeEnum00 = networkBinding.EventTypeEnum00;
                        binding.saveMask |= BindingSaveMask.EventTypeEnum00;
                    }

                    if (binding.EventTypeEnum01 != networkBinding.EventTypeEnum01)
                    {
                        binding.EventTypeEnum01 = networkBinding.EventTypeEnum01;
                        binding.saveMask |= BindingSaveMask.EventTypeEnum01;
                    }

                    if (binding.EventTypeEnum02 != networkBinding.EventTypeEnum02)
                    {
                        binding.EventTypeEnum02 = networkBinding.EventTypeEnum02;
                        binding.saveMask |= BindingSaveMask.EventTypeEnum02;
                    }
                }
            }
        }
    }
}
