using NexusForever.Database.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Internal.Message.Match;
using NexusForever.Server.GroupServer.Group;
using Rebus.Handlers;

namespace NexusForever.Server.GroupServer.Network.Internal.Handler.Match
{
    public class MatchMemberLeftHandler : IHandleMessages<MatchMemberLeftMessage>
    {
        #region Dependency Injection

        private readonly GroupContext _context;
        private readonly GroupManager _groupManager;

        public MatchMemberLeftHandler(
            GroupContext context,
            GroupManager groupManager)
        {
            _context      = context;
            _groupManager = groupManager;
        }

        #endregion

        public async Task Handle(MatchMemberLeftMessage message)
        {
            var group = await _groupManager.GetGroupAsync(message.Match.Guid, message.Team.Team);
            if (group == null)
                return;

            await group.RemoveMemberAsync(message.Member.Identity.ToGroupIdentity(), RemoveReason.Left);

            await _context.SaveChangesAsync();
        }
    }
}
