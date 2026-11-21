using NexusForever.Server.Friendship.Game.Character;
using InternalFriend = NexusForever.Network.Internal.Message.Friendship.Shared.Friend;
using InternalFriendInvite = NexusForever.Network.Internal.Message.Friendship.Shared.FriendInvite;

namespace NexusForever.Server.Friendship.Game.Friend
{
    public static class FriendMappingExtensions
    {
        public static async Task<InternalFriendInvite> ToInternalFriendInvite(this FriendInvite invite)
        {
            return new InternalFriendInvite
            {
                Id               = invite.Id,
                Character        = (await invite.GetInviteeCharacterAsync()).ToInternalCharacter(),
                InviterCharacter = (await invite.GetInviterCharacterAsync()).ToInternalCharacter(),
                //Type             = invite.Type,
                Seen             = invite.Seen,
                Expiration       = invite.Expiration,
                Note             = invite.Note
            };
        }

        public static async Task<InternalFriend> ToInternalFriendAsync(this Friend friend)
        {
            return new InternalFriend
            {
                Id               = friend.Id,
                InviterCharacter = (await friend.GetInviterCharacterAsync()).ToInternalCharacter(),
                InviteeCharacter = (await friend.GetInviteeCharacterAsync()).ToInternalCharacter(),
                Type             = friend.Type,
                Note             = friend.Note
            };
        }
    }
}
