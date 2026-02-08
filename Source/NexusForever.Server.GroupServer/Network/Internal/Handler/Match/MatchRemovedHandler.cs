using NexusForever.Database.Group;
using NexusForever.Network.Internal.Message.Match;
using NexusForever.Network.Internal.Message.Match.Shared;
using NexusForever.Server.GroupServer.Group;
using Rebus.Handlers;

namespace NexusForever.Server.GroupServer.Network.Internal.Handler.Match
{
    public class MatchRemovedHandler : IHandleMessages<MatchRemovedMessage>
    {
        #region Dependency Injection

        private readonly GroupContext _context;
        private readonly GroupManager _groupManager;

        public MatchRemovedHandler(
            GroupContext context,
            GroupManager groupManager)
        {
            _context      = context;
            _groupManager = groupManager;
        }

        #endregion

        public async Task Handle(MatchRemovedMessage message)
        {
            foreach (MatchTeam matchTeam in message.Match.Teams)
            {
                var group = await _groupManager.GetGroupAsync(message.Match.Guid, matchTeam.Team);
                await group?.DisbandAsync();
            }

            await _context.SaveChangesAsync();
        }
    }
}
