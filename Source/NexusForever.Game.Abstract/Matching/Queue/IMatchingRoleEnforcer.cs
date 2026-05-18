namespace NexusForever.Game.Abstract.Matching.Queue
{
    public interface IMatchingRoleEnforcer
    {
        /// <summary>
        /// Check if supplied members meet role enforcement requirements.
        /// </summary>
        IMatchingRoleEnforcerResult Check(IEnumerable<IMatchingQueueProposalMember> members);
    }
}
