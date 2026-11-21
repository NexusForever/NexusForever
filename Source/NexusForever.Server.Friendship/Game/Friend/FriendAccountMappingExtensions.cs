using NexusForever.Server.Friendship.Game.Account;
using InternalFriend = NexusForever.Network.Internal.Message.Friendship.Shared.FriendAccount;
using InternalFriendInvite = NexusForever.Network.Internal.Message.Friendship.Shared.FriendAccountInvite;

namespace NexusForever.Server.Friendship.Game.Friend
{
    public static class FriendAccountMappingExtensions
    {
        public static async Task<InternalFriendInvite> ToInternalFriendInvite(this FriendAccountInvite invite)
        {
            return new InternalFriendInvite
            {
                Id             = invite.Id,
                InviteeAccount = await (await invite.GetInviteeAccountAsync()).ToInternalAccountAsync(),
                InviterAccount = await (await invite.GetInviterAccountAsync()).ToInternalAccountAsync(),
                Seen           = invite.Seen,
                Note           = invite.Note,
                Expiration     = invite.Expiration
            };
        }

        public static async Task<InternalFriend> ToInternalFriendAsync(this FriendAccount friend)
        {
            return new InternalFriend
            {
                Id             = friend.Id,
                InviterAccount = await (await friend.GetInviterAccountAsync()).ToInternalAccountAsync(),
                InviteeAccount = await (await friend.GetInviteeAccountAsync()).ToInternalAccountAsync(),
                Note           = friend.Note
            };
        }
    }
}
