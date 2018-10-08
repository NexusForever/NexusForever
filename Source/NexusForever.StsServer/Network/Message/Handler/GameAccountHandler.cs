using NexusForever.StsServer.Network.Message.Model;

namespace NexusForever.StsServer.Network.Message.Handler
{
    public static class GameAccountHandler
    {
        [MessageHandler("/GameAccount/ListMyAccounts", SessionState.None)]
        public static void HandleListMyAccounts(StsSession session, ListMyAccountsMessage listMyAccounts)
        {
            session.EnqueueMessageOk(new ListMyAccountsResponse
            {

            });
        }
    }
}
