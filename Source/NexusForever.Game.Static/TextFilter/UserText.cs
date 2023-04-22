﻿namespace NexusForever.Game.Static.TextFilter
{
    /// <remarks>
    /// Information for <see cref="UserTextAttribute"/> comes from the text filter manager constructor in the client.
    /// </remarks>
    public enum UserText
    {
        CharacterName                = 0,
        ScientistScanbotName         = 1,
        [UserText(UserTextFlags.NoConsecutiveSpace | UserTextFlags.NoStartEndSpace | UserTextFlags.Unknown20, 30u, 3u)]
        GuildName                    = 2,
        [UserText(UserTextFlags.NoStartEndSpace, 16u, 1u)]
        GuildRankName                = 4,
        GuildBankTabName             = 5,
        [UserText(UserTextFlags.NoConsecutiveSpace | UserTextFlags.NoStartEndSpace | UserTextFlags.Unknown4 | UserTextFlags.Unknown20, 24u, 3u)]
        HousingResidenceName         = 6,
        [UserText(UserTextFlags.None, 500u, 1u)]
        Chat                         = 7,
        MailSubject                  = 8,
        MailBody                     = 9,
        [UserText(UserTextFlags.NoSpace | UserTextFlags.Unknown20, 20u, 1u)]
        ChatCustomChannelName        = 10,
        ReadyCheck                   = 11,
        FriendshipNote               = 12,
        [UserText(UserTextFlags.Unknown80, 20u, 0u)]
        ChatCustomChannelPassword    = 13,
        [UserText(UserTextFlags.AllowMultiline | UserTextFlags.Unknown80, 200u, 0u)]
        GuildMessageOfTheDay         = 14,
        [UserText(UserTextFlags.Unknown80, 32u, 0u)]
        GuildMemberNote              = 15,
        GuildRecruitDescription      = 16,
        [UserText(UserTextFlags.AllowMultiline | UserTextFlags.Unknown80, 400u, 0u)]
        GuildInfoMessage             = 17,
        FriendshipAccountName        = 18,
        FriendshipAccountPrivateNote = 19,
        FriendshipAccountPublicNote  = 20,
        FriendshipAccountEmail       = 21,
        FriendshipInviteNote         = 22,
        PlayerTicketText             = 24,
        PlayerTicketSubject          = 26
    }
}
