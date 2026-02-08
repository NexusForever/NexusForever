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
    public class GroupPlayerInviteHandler : IHandleMessages<GroupPlayerInviteMessage>
    {
        #region Dependency Injection

        private readonly CharacterManager _characterManager;
        private readonly IInternalMessagePublisher _messagePublisher;
        private readonly GroupManager _groupManager;
        private readonly GroupContext _context;

        public GroupPlayerInviteHandler(
            CharacterManager characterManager,
            OutboxMessagePublisher messagePublisher,
            GroupManager groupManager,
            GroupContext context)
        {
            _characterManager = characterManager;
            _messagePublisher = messagePublisher;
            _groupManager     = groupManager;
            _context          = context;
        }

        #endregion

        public async Task Handle(GroupPlayerInviteMessage message)
        {
            IExecutionStrategy strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();

                (GroupInviteResult result, bool isInvite) = await Invite(message);
                if (isInvite)
                {
                    await _messagePublisher.PublishAsync(new GroupPlayerInviteResultMessage
                    {
                        Recipient = message.Inviter,
                        Target = message.Invitee,
                        Result  = result
                    });
                }
                else
                {
                    await _messagePublisher.PublishAsync(new GroupMemberRequestResultMessage
                    {
                        Recipient = message.Inviter,
                        Target    = message.Invitee,
                        Result    = result,
                        Type      = GroupRequestType.Referral
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            });
        }

        private async Task<(GroupInviteResult, bool)> Invite(GroupPlayerInviteMessage message)
        {
            Character.Character inviter = await _characterManager.GetCharacterRemoteAsync(message.Inviter.ToGroupIdentity());
            if (inviter == null)
                return (GroupInviteResult.PlayerNotFound, true);

            IdentityName inviteeIdentity;
            if (string.IsNullOrWhiteSpace(message.Invitee.RealmName))
            {
                inviteeIdentity = new IdentityName
                {
                    Name      = message.Invitee.Name,
                    RealmName = inviter.IdentityName.RealmName
                };
            }
            else
                inviteeIdentity = message.Invitee.ToGroupIdentity();

            Character.Character invitee = await _characterManager.GetCharacterRemoteAsync(inviteeIdentity);
            if (invitee == null)
                return (GroupInviteResult.PlayerNotFound, true);

            if (invitee.PrimaryGroup != null)
                return (GroupInviteResult.Grouped, true);

            if (invitee.GroupInvite != null)
                return (GroupInviteResult.Pending, true);

            if (invitee.Faction != inviter.Faction)
                return (GroupInviteResult.WrongFaction, true);

            if (invitee.Identity == inviter.Identity)
                return (GroupInviteResult.NotInvitingSelf, true);

            if (inviter.PrimaryGroup == null)
            {
                var group = _groupManager.CreateOpenWorldGroup();
                // required to ensure the group has an id assigned before any outbox messages are generated
                await _context.SaveChangesAsync();

                await group.AddMemberAsync(inviter);
                return (await group.InviteMemberAsync(inviter.Identity, invitee), true);
            }
            else
            {
                var group = await inviter.PrimaryGroup.GetGroupAsync();
                var member = group.GetMember(inviter.Identity);

                if (group.Leader == inviter.Identity || member.Flags.HasFlag(GroupMemberInfoFlags.CanInvite))
                    return (await group.InviteMemberAsync(inviter.Identity, invitee), true);
                else
                    return (await group.ReferMemberAsync(inviter.Identity, invitee), false);
            }
        }
    }
}
