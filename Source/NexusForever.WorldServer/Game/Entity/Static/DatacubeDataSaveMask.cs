using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    [Flags]
    public enum DatacubeDataSaveMask
    {
        None            = 0x0000,
        Datacube        = 0x0001,
        DatacubeVolume  = 0x0002,
    }
}
