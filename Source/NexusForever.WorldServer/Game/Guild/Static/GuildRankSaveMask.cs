using System;

namespace NexusForever.WorldServer.Game.Guild.Static
{
    /// <summary>
    /// Determines which fields need saving for <see cref="GuildRank"/> when being saved to the database.
    /// </summary>
    [Flags]
    public enum GuildRankSaveMask
    {
        None                     = 0x0000,
        Create                   = 0x0001,
        Delete                   = 0x0002,
        //Index                    = 0x0004,
        Name                     = 0x0008,
        Permissions              = 0x0010,
        BankPermissions          = 0x0020,
        BankMoneyWithdrawlLimits = 0x0040,
        RepairLimit              = 0x0080
    }
}
