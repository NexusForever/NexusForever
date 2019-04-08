using System;

namespace NexusForever.WorldServer.Game.Setting.Static
{
    [Flags]
    public enum BindingSaveMask
    {
        None            = 0x0000,
        Create          = 0x0001,
        Delete          = 0x0002,
        DeviceEnum00    = 0x0004,
        DeviceEnum01    = 0x0008, 
        DeviceEnum02    = 0x0010, 
        Code00          = 0x0020,
        Code01          = 0x0040,
        Code02          = 0x0080,
        MetaKeys00      = 0x0100,
        MetaKeys01      = 0x0200,
        MetaKeys02      = 0x0400,
        EventTypeEnum00 = 0x0800,
        EventTypeEnum01 = 0x1000,
        EventTypeEnum02 = 0x2000
    }
}
