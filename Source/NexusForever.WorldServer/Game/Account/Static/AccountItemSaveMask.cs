using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Account.Static
{
    [Flags]
    public enum AccountItemSaveMask
    {
        None   = 0x0000,
        Create = 0x0001,
        Delete = 0x0002
    }
}
