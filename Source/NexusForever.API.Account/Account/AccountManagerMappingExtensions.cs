using NexusForever.Database.Auth.Model;

namespace NexusForever.API.Account.Account
{
    public static class AccountManagerMappingExtensions
    {
        public static Model.Account.Account ToAccount(this AccountModel account)
        {
            return new Model.Account.Account
            {
                Id    = account.Id,
                Email = account.Email
            };
        }
    }
}
