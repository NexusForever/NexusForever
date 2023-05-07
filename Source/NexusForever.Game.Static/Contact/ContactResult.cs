namespace NexusForever.Game.Static.Contact
{
    public enum ContactResult
    {
        Ok = -1,                    // Used by the Server (not understood by the client)
        PlayerNotFound,             // Invite canceled: No such player
        RealmNotFound,              // Invite canceled: No such realm.
        RequestDenied,              // Request denied. They sure didn't look very friendly!
        PlayerAlreadyFriend,        // Invite canceled: You'Re already Friends. You must really like them.
        PlayerOffline,              // Player Offline.
        FriendshipNotFound,         // Not found.
        InvalidType,                // Invalid Type.
        RequestNotFound,            // Request not found.
        RequestTimedOut,            // Request Timed Out.
        Busy,                       // Busy. Try again in a bit.
        InvalidNote,                // Invalid Note.
        MaxFriends,                 // Max Friend Count. Somebody's popular!
        MaxRivals,                  // Max Rival Count. Somebody's got issues!
        MaxIgnored,                 // Max Ignored Count.
        UnableToProcess,            // Unable to Process.
        PlayerNotFriend,            // Invite canceled: No such player.
        PlayerQueuedRequests,       // Player considering other friendships. Try again in a bit.
        RequestSent,                // Friend invited.
        PlayerAlreadyRival,         // Already a Rival.
        PlayerAlreadyNeighbor,      // Player Already a Neighbor.
        PlayerAlreadyIgnored,       // Player already ignored. They're that bad, huh?
        PlayerOnIgnored,            // Player Ignored.
        PlayerNotRival,             // Player not a Rival
        PlayerNotIgnored,           // Player not Ignored
        PlayerNotNeighbor,          // Player not a Neighbor
        PlayerNotOfThisRealm,       // Player is not member of this realm.
        FriendsBlocked,             // Player is blocking Friend Requests. Mean, right?
        // 27 missing
        LocationsBusy = 28,         // 
        CannotInviteSelf = 29,      // You can't ignore or add yourself as a Friend or Rival.
        ThrottledRequest = 30,      // Please try again, and slow down!
        ContainsProfanity = 31,     // This text contains profanity.
        InvalidPublicNote = 32,     // Invalid public note
        InvalidDisplayName = 33,    // Invalid display name
        InvalidEmail = 34,          // Invalid display name
        InvalidInviteNote = 35,     //
        BlockedForStrangers = 36,   // Request is blocked for strangers
        InvalidAutoResponse = 37,   // Invalid auto-response
        TrialAccountDenied = 38,    // Trial account can't add other players to their Friends list.
        NameUnavailable = 39,       // That name is unavailable.
        PrivilegesSuspended = 40,   // Your friend invite privileges have temporarily been restricted.
    }
}
