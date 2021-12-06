namespace NexusForever.Game.Static.Group
{
    public enum GroupInviteResult
    {
        /// <summary>
        /// Player declined the Invite
        /// </summary>
        Declined = 0,

        /// <summary>
        /// Player accepted the Invite
        /// </summary>
        Accepted = 1,

        /// <summary>
        /// Player ran out of time to accept Invite
        /// </summary>
        OutOfTime = 2,
    }
}
