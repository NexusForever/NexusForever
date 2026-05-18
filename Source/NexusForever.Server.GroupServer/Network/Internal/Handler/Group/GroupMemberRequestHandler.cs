using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NexusForever.Database.Group;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Internal;
using NexusForever.Network.Internal.Message.Group;
using NexusForever.Server.GroupServer.Character;
using NexusForever.Server.GroupServer.Group;
using Rebus.Handlers;

namespace NexusForever.Server.GroupServer.Network.Internal.Handler.Group
{
    public class GroupMemberRequestHandler : IHandleMessages<GroupMemberRequestMessage>
    {
        #region Dependency Injection

        private readonly GroupContext _context;
        private readonly CharacterManager _characterManager;
        private readonly GroupManager _groupManager;
        private readonly IInternalMessagePublisher _messagePublisher;

        public GroupMemberRequestHandler(
            GroupContext context,
            CharacterManager characterManager,
            GroupManager groupManager,
            IInternalMessagePublisher messagePublisher)
        {
            _context          = context;
            _characterManager = characterManager;
            _groupManager     = groupManager;
            _messagePublisher = messagePublisher;
        }

        #endregion

        public async Task Handle(GroupMemberRequestMessage message)
        {
            IExecutionStrategy strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();

                (GroupInviteResult result, bool isRequest) = await Request(message);
                if (isRequest)
                {
                    await _messagePublisher.PublishAsync(new GroupMemberRequestResultMessage
                    {
                        Recipient = message.Requester,
                        Target    = message.Requestee,
                        Result    = result,
                        Type      = GroupRequestType.Request
                    });
                }
                else
                {
                    await _messagePublisher.PublishAsync(new GroupPlayerInviteResultMessage
                    {
                        Recipient = message.Requester,
                        Target    = message.Requestee,
                        Result    = result
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            });
        }

        private async Task<(GroupInviteResult Result, bool IsRequest)> Request(GroupMemberRequestMessage message)
        {
            Character.Character source = await _characterManager.GetCharacterRemoteAsync(message.Requester.ToGroupIdentity());
            if (source == null)
                return (GroupInviteResult.PlayerNotFound, true);

            if (source.PrimaryGroup != null)
                return (GroupInviteResult.Grouped, true);

            IdentityName inviteeIdentity;
            if (string.IsNullOrWhiteSpace(message.Requestee.RealmName))
            {
                inviteeIdentity = new IdentityName
                {
                    Name      = message.Requestee.Name,
                    RealmName = source.IdentityName.RealmName
                };
            }
            else
                inviteeIdentity = message.Requestee.ToGroupIdentity();

            Character.Character target = await _characterManager.GetCharacterRemoteAsync(inviteeIdentity);
            if (target == null)
                return (GroupInviteResult.PlayerNotFound, true);

            if (source.Identity == target.Identity)
                return (GroupInviteResult.NotInvitingSelf, true);

            if (target.PrimaryGroup == null)
            {
                var group = _groupManager.CreateOpenWorldGroup();
                // required to ensure the group has an id assigned before any outbox messages are generated
                await _context.SaveChangesAsync();

                await group.AddMemberAsync(source);
                return (await group.InviteMemberAsync(source.Identity, target), false);
            }
            else
            {
                var group = await target.PrimaryGroup.GetGroupAsync();
                if (group == null)
                    return (GroupInviteResult.GroupNotFound, true);

                return (await group.RequestMemberAsync(source), true);
            }
        }
    }
}
