using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Account
{
    [Flags]
    public enum AccountCurrencySaveMask
    {
        None    = 0x0000,
        Create  = 0x0001,
        Amount  = 0x0002
    }
}
