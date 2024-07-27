using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Game.Static.Matching;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared;
using NexusForever.Shared.Game;

namespace NexusForever.Game.Matching.Queue
{
    public class MatchingRoleCheck : IMatchingRoleCheck
    {
        public Guid Guid { get; } = Guid.NewGuid();
        public MatchingRoleCheckStatus Status { get; private set; }
        public IMatchingQueueProposal MatchingQueueProposal { get; private set; }

        private readonly Dictionary<ulong, IMatchingRoleCheckMember> members = [];
        private UpdateTimer expiryTimer;

        #region Dependency Injection

        private readonly ILogger<MatchingRoleCheck> log;
        private readonly IFactory<IMatchingRoleCheckMember> matchingRoleCheckMemberFactory;

        public MatchingRoleCheck(
            ILogger<MatchingRoleCheck> log,
            IFactory<IMatchingRoleCheckMember> matchingRoleCheckMemberFactory)
        {
            this.log                            = log;
            this.matchingRoleCheckMemberFactory = matchingRoleCheckMemberFactory;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="IMatchingRoleCheck"/> with the supplied <see cref="IMatchingQueueProposal"/> and character ids.
        /// </summary>
        public void Initialise(IMatchingQueueProposal matchingQueueProposal, List<ulong> characterIds)
        {
            if (MatchingQueueProposal != null)
                throw new InvalidOperationException();

            MatchingQueueProposal = matchingQueueProposal;

            foreach (ulong characterId in characterIds)
            {
                IMatchingRoleCheckMember matchingRoleCheckMember = matchingRoleCheckMemberFactory.Resolve();
                matchingRoleCheckMember.Initialise(characterId);
                members.Add(characterId, matchingRoleCheckMember);
            }

            SendMatchingRoleCheckStarted();

            expiryTimer = new UpdateTimer(TimeSpan.FromSeconds(30d));

            log.LogTrace($"MatchingRoleCheck {Guid} has started, waiting for {members.Count} to respond.");
        }

        /// <summary>
        /// Invoked each world tick with the delta since the previous tick occurred.
        /// </summary>
        public void Update(double lastTick)
        {
            if (!expiryTimer.IsTicking)
                return;

            expiryTimer.Update(lastTick);
            if (expiryTimer.HasElapsed)
            {
                SendMatchingRoleCheckCancelled();

                Status = MatchingRoleCheckStatus.Expired;
                log.LogTrace($"MatchingRoleCheck {Guid} has expired.");
            }
        }

        public IEnumerable<IMatchingRoleCheckMember> GetMembers()
        {
            return members.Values;
        }

        /// <summary>
        /// Set role response for supplied character id and <see cref="Role"/>.
        /// </summary>
        /// <remarks>
        /// This will also update the <see cref="MatchingRoleCheckStatus"/> to <see cref="MatchingRoleCheckStatus.Success"/> if all members have responded or <see cref="MatchingRoleCheckStatus.Declined"/> if any member has declined.
        /// </remarks>
        public void Respond(ulong characterId, Role roles)
        {
            if (Status != MatchingRoleCheckStatus.Pending)
                throw new InvalidOperationException();

            if (!members.TryGetValue(characterId, out IMatchingRoleCheckMember matchingRoleCheckMember))
                throw new InvalidOperationException();

            matchingRoleCheckMember.SetRoles(roles);

            if (roles == Role.None)
            {
                SendMatchingRoleCheckCancelled();

                Status = MatchingRoleCheckStatus.Declined;
                log.LogTrace($"MatchingRoleCheck {Guid} has been declined.");
            }
            else
            {
                uint pendingMembers = (uint)members.Values.Count(m => m.Roles.HasValue);
                log.LogTrace($"MatchingRoleCheck {Guid} received role {roles} response from {characterId}, waiting for {members.Count - pendingMembers} to respond.");

                if (pendingMembers == members.Count)
                {
                    Status = MatchingRoleCheckStatus.Success;
                    log.LogTrace($"MatchingRoleCheck {Guid} has been completed.");
                }
            }
        }

        private void SendMatchingRoleCheckStarted()
        {
            foreach (IMatchingRoleCheckMember member in members.Values)
            {
                member.Send(new ServerMatchingRoleCheckStarted
                {
                    // this will change the UI to prompt the client for a role selection or just accept or decline
                    RolesRequired = !member.Roles.HasValue
                });
            }
        }

        private void SendMatchingRoleCheckCancelled()
        {
            Broadcast(new ServerMatchingQueueResult
            {
                Result = MatchingQueueResult.Role
            });
        }

        private void Broadcast(IWritable message)
        {
            foreach (IMatchingRoleCheckMember matchingRoleCheckMember in members.Values)
                matchingRoleCheckMember.Send(message);
        }
    }
}
