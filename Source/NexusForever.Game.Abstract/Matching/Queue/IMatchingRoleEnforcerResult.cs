namespace NexusForever.Game.Abstract.Matching.Queue
{
    public interface IMatchingRoleEnforcerResult
    {
        bool Success { get; set; }
        List<IMatchingRoleEnforcerResultMember> Members { get; }
    }
}
