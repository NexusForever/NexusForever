using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NexusForever.Database.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Game.Static.Matching;
using NexusForever.Network.Internal.Message.Match;
using NexusForever.Network.Internal.Message.Match.Shared;
using NexusForever.Server.GroupServer.Group;
using Rebus.Handlers;

namespace NexusForever.Server.GroupServer.Network.Internal.Handler.Match
{
    public class MatchCreatedHandler : IHandleMessages<MatchCreatedMessage>
    {
        #region Dependency Injection

        private readonly GroupContext _context;
        private readonly GroupManager _groupManager;

        public MatchCreatedHandler(
            GroupContext context,
            GroupManager groupManager)
        {
            _context      = context;
            _groupManager = groupManager;
        }

        #endregion

        public async Task Handle(MatchCreatedMessage message)
        {
            IExecutionStrategy strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();

                foreach (var matchTeam in message.Match.Teams)
                {
                    var group = _groupManager.CreateInstanceGroup();
                    group.Match     = message.Match.Guid;
                    group.MatchTeam = matchTeam.Team;
                    await _context.SaveChangesAsync();

                    foreach (MatchTeamMember matchTeamMember in matchTeam.Members)
                    {
                        GroupMember member = await group.AddMemberAsync(matchTeamMember.Identity.ToGroupIdentity());
                        if (member == null)
                            continue;

                        var flags = GroupMemberInfoFlags.None;
                        if (matchTeamMember.Roles.HasFlag(Role.Tank))
                            flags |= GroupMemberInfoFlags.Tank;
                        if (matchTeamMember.Roles.HasFlag(Role.Healer))
                            flags |= GroupMemberInfoFlags.Healer;
                        if (matchTeamMember.Roles.HasFlag(Role.DPS))
                            flags |= GroupMemberInfoFlags.DPS;

                        if (flags != GroupMemberInfoFlags.None)
                            await member.SetFlagAsync(flags);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            });
        }
    }
}
