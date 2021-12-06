namespace NexusForever.Game.Static.Group
{
    /// <summary>
    /// Indicates how person is joining the group
    /// </summary>
    public enum GroupInviteType
    {
        /// <summary>
        /// Person is being invited into group by raid lead / assistant
        /// </summary>
        Invite,
        /// <summary>
        /// Person requests to join the group
        /// </summary>
        Request,
        /// <summary>
        /// Person is being referred to join by another group member
        /// </summary>
        Referral
    }
}
