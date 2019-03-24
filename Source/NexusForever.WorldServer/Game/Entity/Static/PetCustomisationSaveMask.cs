using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    [Flags]
    public enum PetCustomisationSaveMask
    {
        None   = 0x00,
        Create = 0x01,
        Name   = 0x02,
        Flairs = 0x04
    }
}
