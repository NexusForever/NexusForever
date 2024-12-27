using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Game.Static.Matching;
using NexusForever.Shared;

namespace NexusForever.Game.Matching.Queue
{
    public class MatchingRoleEnforcer : IMatchingRoleEnforcer
    {
        private const uint MaxTank   = 1u;
        private const uint MaxHealer = 1u;
        private const uint MaxDps    = 3u;

        #region Dependency Injection

        private readonly IFactory<IMatchingRoleEnforcerResult> resultFactory;

        public MatchingRoleEnforcer(
            IFactory<IMatchingRoleEnforcerResult> resultFactory)
        {
            this.resultFactory = resultFactory;
        }

        #endregion

        /// <summary>
        /// Check if supplied members meet role enforcement requirements.
        /// </summary>
        public IMatchingRoleEnforcerResult Check(IEnumerable<IMatchingQueueProposalMember> members)
        {
            IMatchingRoleEnforcerResult result = resultFactory.Resolve();

            foreach (IMatchingQueueProposalMember member in members)
            {
                result.Members.Add(new MatchingRoleEnforcerResultMember
                {
                    CharacterId = member.CharacterId,
                    Role        = member.Roles
                });
            }

            Check(result);
            return result;
        }

        private bool Check(IMatchingRoleEnforcerResult result)
        {
            uint tankCount   = 0u;
            uint healerCount = 0u;
            uint dpsCount    = 0u;

            foreach (IMatchingRoleEnforcerResultMember resultMember in result.Members)
            {
                bool? tankResult = CheckRole(result, ref tankCount, MaxTank, resultMember, Role.Tank);
                if (tankResult.HasValue)
                    return tankResult.Value;

                bool? healerResult = CheckRole(result, ref healerCount, MaxHealer, resultMember, Role.Healer);
                if (healerResult.HasValue)
                    return healerResult.Value;

                bool? dpsResult = CheckRole(result, ref dpsCount, MaxDps, resultMember, Role.DPS);
                if (dpsResult.HasValue)
                    return dpsResult.Value;
            }

            result.Success = (tankCount + healerCount + dpsCount) == result.Members.Count;
            return result.Success;
        }

        private bool? CheckRole(IMatchingRoleEnforcerResult result, ref uint count, uint max, IMatchingRoleEnforcerResultMember member, Role role)
        {
            if (member.Role.HasFlag(role))
            {
                if (member.Role != role)
                {
                    member.Role &= ~role;
                    if (Check(result))
                        return true;
                    member.Role |= role;
                }
                else if (count == max)
                    return false;
                else
                    count++;
            }

            return null;
        }
    }
}
