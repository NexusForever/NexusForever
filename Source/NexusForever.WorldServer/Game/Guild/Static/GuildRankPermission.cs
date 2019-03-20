using System;

namespace NexusForever.WorldServer.Game.Guild.Static
{
    [Flags]
    public enum GuildRankPermission : long
    {
        Leader                      = -2,
        None                        = 0,
        Disabled                    = 1,
        ReserveCommunityPlot        = 2,
        CreateAndRemoveRank         = 4,
        EditLowerRankPermissions    = 8,
        SpendInfluence              = 16,
        RenameRank                  = 32,
        Vote                        = 64,
        ChangeMemberRank            = 128,
        Invite                      = 256,
        Kick                        = 512,
        EditGuildHolomark           = 1024,
        MemberChat                  = 2048,
        OfficerChat                 = 4096,
        BankTabRename               = 8192,
        RemoveCommunityPlotReservation = 16384,
        ActivateWarPlotPlugs        = 32768,
        AddWarPlotDeployables       = 65536,
        AddWarPlotPlugs             = 131072,
        UpgradeWarPlotPlugs         = 162144,
        RemoveWarPlotPlugs          = 524288,
        RepairWarPlotPlugsOutOfMatch = 1048576,
        RepairWarPlotPlugsInMatch   = 2097152,
        InitiateWarPlotSurrenderVotes = 4194304,
        QueueTheWarPlot             = 8388608,
        UseWarPlotVehicles          = 16777216,
        KickPlayersfromWarPlot      = 33554432,
        MessageOfTheDay             = 134217728,
        DeleteCommunityDecor        = 268435456,
        BankTabLog                  = 536870912,
        DecorateCommunity           = 1073741824,
        ChangeCommunityRemodelOptions = 2147483648,
    }
}
