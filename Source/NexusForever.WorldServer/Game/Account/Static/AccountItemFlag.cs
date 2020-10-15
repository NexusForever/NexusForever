using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Account.Static
{
    [Flags]
    public enum AccountItemFlag
    {
        None        = 0x0000,
        MultiClaim  = 0x0002
    }
}
