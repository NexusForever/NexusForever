using System;
using NexusForever.Shared.Database;
using NexusForever.Shared.Database.Auth.Model;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Setting.Static;
using NetworkBinding = NexusForever.WorldServer.Network.Message.Model.Shared.Binding;

namespace NexusForever.WorldServer.Game.Setting
{
    public class Binding
    {
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

        public BindingSaveMask saveMask { get; set; }
        
        public Binding(bool init = false)
        {
            if (!init)
                saveMask = BindingSaveMask.Create;
        }

        public Binding(AccountKeybinding model)
        {
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
        
        public Binding(CharacterKeybinding model)
        {
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
        public Binding(NetworkBinding networkBinding)
        {
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

        public void Keep()
        {
            saveMask |= BindingSaveMask.Keep;
        }
    }
}
