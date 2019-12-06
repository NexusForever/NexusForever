using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.RewardTrack.Static
{
    [Flags]
    public enum RewardTrackSaveMask
    {
        None    = 0x0000,
        Create  = 0x0001,
        Delete  = 0x0002,
        Modify  = 0x0004
    }
}
