using NexusForever.API.Account.Account;

namespace NexusForever.API.Account.Endpoint
{
    public static class GetAccountEndpount
    {
        public static void MapGetAccountEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/account/{id:int}", async (AccountManager accountManager, uint id) =>
            {
                Model.Account.Account account = await accountManager.GetAccountAsync(id);
                if (account == null)
                    return Results.NotFound();

                return Results.Ok(account);
            });

            app.MapGet("/account/{email}", async (AccountManager accountManager, string email) =>
            {
                Model.Account.Account account = await accountManager.GetAccountAsync(email);
                if (account == null)
                    return Results.NotFound();

                return Results.Ok(account);
            });
        }
    }
}
