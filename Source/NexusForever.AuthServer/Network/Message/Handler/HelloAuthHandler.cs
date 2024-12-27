using System;
using System.Linq;
using NexusForever.Cryptography;
using NexusForever.Database;
using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.Game.Abstract.Server;
using NexusForever.Network.Auth.Message.Model;
using NexusForever.Network.Auth.Static;
using NexusForever.Network.Message;
using NexusForever.Shared.Game.Events;
using NetworkMessage = NexusForever.Network.Message.Model.Shared.Message;

namespace NexusForever.AuthServer.Network.Message.Handler
{
    public class HelloAuthHandler : IMessageHandler<AuthSession, ClientHelloAuth>
    {
        #region Dependency Injection

        private readonly IDatabaseManager databaseManager;
        private readonly IServerManager serverManager;

        public HelloAuthHandler(
            IDatabaseManager databaseManager,
            IServerManager serverManager)
        {
            this.databaseManager = databaseManager;
            this.serverManager   = serverManager;
        }

        #endregion

        public void HandleMessage(AuthSession session, ClientHelloAuth helloAuth)
        {
            void SendServerAuthDenied(NpLoginResult result)
            {
                session.EnqueueMessageEncrypted(new ServerAuthDenied
                {
                    LoginResult = result
                });
            }

            void SendServerAuthDeniedSuspended(NpLoginResult result, float suspendedDays)
            {
                session.EnqueueMessageEncrypted(new ServerAuthDenied
                {
                    LoginResult   = result,
                    SuspendedDays = suspendedDays
                });
            }

            if (helloAuth.Build != 16042)
            {
                SendServerAuthDenied(NpLoginResult.ClientServerVersionMismatch);
                return;
            }

            string gameToken = Convert.ToHexString(helloAuth.GameToken.Guid.ToByteArray());
            session.Events.EnqueueEvent(new TaskGenericEvent<AccountModel>(databaseManager.GetDatabase<AuthDatabase>().GetAccountByGameTokenAsync(helloAuth.Email, gameToken),
                account =>
            {
                if (account == null)
                {
                    SendServerAuthDenied(NpLoginResult.ErrorInvalidToken);
                    return;
                }

                if (account.AccountSuspension.Any(a => a.EndTime == null))
                {
                    SendServerAuthDenied(NpLoginResult.ErrorAccountBanned);
                    return;
                }

                DateTime? latestSuspension = account.AccountSuspension.Max(suspension => suspension.EndTime);
                if (latestSuspension != null && latestSuspension > DateTime.Now)
                {
                    SendServerAuthDeniedSuspended(NpLoginResult.AccountSuspended, (float)((DateTime)latestSuspension - DateTime.Now).TotalDays);
                    return;
                }

                // TODO: might want to make this smarter in the future, eg: select a server the user has characters on
                IServerInfo server = serverManager.Servers.FirstOrDefault(s => s.IsOnline);
                if (server == null)
                {
                    SendServerAuthDenied(NpLoginResult.NoRealmsAvailableAtThisTime);
                    return;
                }

                session.EnqueueMessageEncrypted(new ServerAuthAccepted());
                session.EnqueueMessageEncrypted(new ServerRealmMessages
                {
                    Messages = serverManager.ServerMessages
                        .Select(m => new NetworkMessage
                        {
                            Index    = m.Index,
                            Messages = m.Messages
                        })
                        .ToList()
                });

                byte[] sessionKey = RandomProvider.GetBytes(16u);

                account.SessionKey = Convert.ToHexString(sessionKey);
                session.Events.EnqueueEvent(new TaskEvent(databaseManager.GetDatabase<AuthDatabase>().UpdateAccountSessionKey(account.Id, account.SessionKey),
                    () =>
                {
                    session.EnqueueMessageEncrypted(new ServerRealmInfo
                    {
                        AccountId  = account.Id,
                        SessionKey = sessionKey,
                        Realm      = server.Model.Name,
                        Address    = server.Address,
                        Port       = server.Model.Port,
                        Type       = server.Model.Type
                    });
                }));
            }));
        }
    }
}
