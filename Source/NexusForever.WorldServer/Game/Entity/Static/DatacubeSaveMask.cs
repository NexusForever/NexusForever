using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    [Flags]
    public enum DatacubeSaveMask
    {
        None     = 0x0000,
        Create   = 0x0001,
        Progress = 0x0002
    }
}
