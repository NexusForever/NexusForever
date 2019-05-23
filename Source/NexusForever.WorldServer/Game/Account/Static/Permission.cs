using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Account.Static
{
    public enum Permission: long
    {
        Everything                  = -1,
        None                        = 0,
        CommandAccountCreate        = 1,
        CommandAccountDelete        = 2,
        CommandBroadcast            = 3,
        CommandGenericUnlock        = 4,
        CommandGenericUnlockAll     = 5,
        CommandGenericList          = 6,
        CommandGo                   = 7,
        CommandItemAdd              = 8,
        CommandMovementSplineAdd    = 9,
        CommandMovementSplineClear  = 10,
        CommandMovementSplineLaunch = 11,
        CommandPathActivate         = 12,
        CommandPathUnlock           = 13,
        CommandPathAddXp            = 16,
        CommandPetUnlockFlair       = 17,
        CommandSpellAdd             = 18,
        CommandSpellCast            = 19,
        CommandSpellResetCooldowns  = 20,
        CommandTeleport             = 21,
        CommandTitleAdd             = 22,
        CommandTitleRevoke          = 23,
        CommandTitleAll             = 24,
        CommandTitleNone            = 25,
        CommandHouseTeleport        = 26,
        CommandHouseDecorAdd        = 27,
        CommandHouseDecorLookup     = 28,
        CommandLocation             = 29,
        GMFlag                      = 100,
    }
}
