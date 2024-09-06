namespace NexusForever.Game.Static.Group
{
    public enum GroupResult
    {
        Sent                    = 0,
        Declined                = 1,
        Accepted                = 2,
        NoPermissions           = 3,
        PlayerNotFound          = 5,
        Grouped                 = 6,
        Pending                 = 7,
        ExpiredInviter          = 8,
        IsInvited               = 9,
        ExpiredInvitee          = 10,
        InvitedYou              = 11,
        RealmNotFound           = 12,
        Full                    = 13,
        RoleFull                = 14,
        NotInvitingSelf         = 15,
        ServerControlled        = 16,
        GroupNotFound           = 17,
        NotAcceptingRequests    = 18,
        Busy                    = 19,
        SentToLeader            = 20,
        LeaderOffline           = 21,
        WrongFaction            = 22,
        PrivilegeRestricted     = 23,
        PvpFlagRestriction      = 24
    }
}
