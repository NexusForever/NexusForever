using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Account.Static
{
    public enum AccountStatus
    {
        Player              = 1,
        TrustedPlayer       = 2,
        GameMaster          = 3,
        SeniorGameMaster    = 4,
        Developer           = 5,
        Administrator       = 6
    }
}
