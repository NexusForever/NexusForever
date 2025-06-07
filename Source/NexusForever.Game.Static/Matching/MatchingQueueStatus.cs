
namespace NexusForever.Game.Static.Matching
{
    public enum MatchingQueueStatus
    {
        NotInQueue                                  = 0, // no effect on client
        InQueue                                     = 1, // cancels role check on client
        MatchReady                                  = 2, // no effect on client
        GroupQueuedWaitingPlayerRoleSelection       = 3, // no effect on client
        GroupQueuedWaitingPartyMemberRoleSelection  = 4, // no effect on client
        WaitingAllParticipantsToAccept              = 5, // no effect on client
        MatchJoined                                 = 6, // cancels role check on client

        // Values 7 to 9 have no effect on client
        // Values 10 to 15 causes MatchLeft event and cancel role check on client
    }
}
