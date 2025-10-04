using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NexusForever.Database.Group;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Server.GroupServer.Group;
using Rebus.Handlers;

namespace NexusForever.Server.GroupServer.Network.Internal.Handler.Group
{
    public class GroupMemberRequestReponseHandler : IHandleMessages<GroupMemberRequestReponseMessage>
    {
        #region Dependency Injection

        private readonly GroupContext _context;
        private readonly GroupManager _groupManager;

        public GroupMemberRequestReponseHandler(
            GroupContext context,
            GroupManager groupManager)
        {
            _context      = context;
            _groupManager = groupManager;
        }

        #endregion

        public async Task Handle(GroupMemberRequestReponseMessage message)
        {
            IExecutionStrategy strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                // use serialisable isolation level to lock the selected group to prevent multiple members from being added at the same time
                await using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

                var group = await _groupManager.GetGroupAsync(message.GroupId);
                if (group == null)
                    return;

                await group.RequestMemberResponseAsync(message.Identity.ToGroupIdentity(), message.Response);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            });
        }
    }
}
