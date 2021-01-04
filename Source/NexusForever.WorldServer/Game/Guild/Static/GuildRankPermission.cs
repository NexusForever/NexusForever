using System;

namespace NexusForever.WorldServer.Game.Guild.Static
{
    [Flags]
    public enum GuildRankPermission : uint
    {
        None                           = 0x00000000,
        Disabled                       = 0x00000001,
        ReserveCommunityPlot           = 0x00000002,
        CreateAndRemoveRank            = 0x00000004,
        EditLowerRankPermissions       = 0x00000008,
        SpendInfluence                 = 0x00000010,
        RenameRank                     = 0x00000020,
        Vote                           = 0x00000040,
        ChangeMemberRank               = 0x00000080,
        Invite                         = 0x00000100,
        Kick                           = 0x00000200,
        EditGuildHolomark              = 0x00000400,
        MemberChat                     = 0x00000800,
        OfficerChat                    = 0x00001000,
        BankTabRename                  = 0x00002000,
        RemoveCommunityPlotReservation = 0x00004000,
        ActivateWarPlotPlugs           = 0x00008000,
        AddWarPlotDeployables          = 0x00010000,
        AddWarPlotPlugs                = 0x00020000,
        //UpgradeWarPlotPlugs            = 162144,
        RemoveWarPlotPlugs             = 0x00080000,
        RepairWarPlotPlugsOutOfMatch   = 0x00100000,
        RepairWarPlotPlugsInMatch      = 0x00200000,
        InitiateWarPlotSurrenderVotes  = 0x00400000,
        QueueTheWarPlot                = 0x00800000,
        UseWarPlotVehicles             = 0x01000000,
        KickPlayersfromWarPlot         = 0x02000000,
        MessageOfTheDay                = 0x08000000,
        DeleteCommunityDecor           = 0x10000000,
        BankTabLog                     = 0x20000000,
        DecorateCommunity              = 0x40000000,
        ChangeCommunityRemodelOptions  = 0x80000000,

        // TODO: this might need to be moved, different guild types might have different default permissions
        Leader                         = 0xFFFFFFFF - Disabled,
        Council                        = OfficerChat | MemberChat | Kick | Invite | ChangeMemberRank | Vote
    }
}
