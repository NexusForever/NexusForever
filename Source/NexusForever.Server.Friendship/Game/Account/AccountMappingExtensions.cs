using NexusForever.Database.Friendship.Model;
using NexusForever.Server.Friendship.Game.Character;
using InternalAccount = NexusForever.Network.Internal.Message.Friendship.Shared.Account;

namespace NexusForever.Server.Friendship.Game.Account
{
    public static class AccountMappingExtensions
    {
        /// <summary>
        /// Convert an <see cref="Account"/> to an internal message model.
        /// </summary>
        /// <param name="account">The <see cref="Account"/> to convert to the internal message model.</param>
        /// <returns>The internal message model of the <see cref="Account"/>.</returns>
        public static async Task<InternalAccount> ToInternalAccountAsync(this Account account)
        {
            return new InternalAccount
            {
                Id                         = account.Id,
                Email                      = account.Email,
                Nickname                   = account.Nickname,
                Status                     = account.Status,
                Presence                   = account.Presence,
                BlockAccountFriendRequests = account.BlockAccountFriendRequests,
                ActiveCharacter            = (await account.GetActiveCharacterAsync())?.ToInternalCharacter(),
                LastOnline                 = account.LastOnline
            };
        }

        public static AccountModel ToDatabaseAccount(this API.Model.Account.Account account)
        {
            return new AccountModel
            {
                AccountId = account.Id,
                Email     = account.Email
            };
        }
    }
}
