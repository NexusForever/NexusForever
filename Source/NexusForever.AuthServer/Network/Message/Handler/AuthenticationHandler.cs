using System.Linq;
using NexusForever.AuthServer.Network.Message.Model;
using NexusForever.AuthServer.Network.Message.Static;
using NexusForever.Database.Auth.Model;
using NexusForever.Shared;
using NexusForever.Shared.Cryptography;
using NexusForever.Shared.Database;
using NexusForever.Shared.Game;
using NexusForever.Shared.Game.Events;
using NexusForever.Shared.Network.Message;
using NetworkMessage = NexusForever.Shared.Network.Message.Model.Shared.Message;

namespace NexusForever.AuthServer.Network.Message.Handler
{
    public static class AuthenticationHandler
    {
        [MessageHandler(GameMessageOpcode.ClientHelloAuth)]
        public static void HandleHelloAuth(AuthSession session, ClientHelloAuth helloAuth)
        {
            void SendServerAuthDenied(NpLoginResult result)
            {
                session.EnqueueMessageEncrypted(new ServerAuthDenied
                {
                    LoginResult = result
                });
            }

            if (helloAuth.Build != 16042)
            {
                SendServerAuthDenied(NpLoginResult.ClientServerVersionMismatch);
                return;
            }

            string gameToken = helloAuth.GameToken.Guid
                .ToByteArray()
                .ToHexString();
            session.EnqueueEvent(new TaskGenericEvent<AccountModel>(DatabaseManager.Instance.AuthDatabase.GetAccountByGameTokenAsync(helloAuth.Email, gameToken),
                account =>
            {
                if (account == null)
                {
                    SendServerAuthDenied(NpLoginResult.ErrorInvalidToken);
                    return;
                }

                // TODO: might want to make this smarter in the future, eg: select a server the user has characters on
                ServerInfo server = ServerManager.Instance.Servers.FirstOrDefault(s => s.IsOnline);
                if (server == null)
                {
                    SendServerAuthDenied(NpLoginResult.NoRealmsAvailableAtThisTime);
                    return;
                }

                session.EnqueueMessageEncrypted(new ServerAuthAccepted());
                session.EnqueueMessageEncrypted(new ServerRealmMessages
                {
                    Messages = ServerManager.Instance.ServerMessages
                        .Select(m => new NetworkMessage
                        {
                            Index    = m.Index,
                            Messages = m.Messages
                        })
                        .ToList()
                });

                byte[] sessionKey = RandomProvider.GetBytes(16u);
                session.EnqueueEvent(new TaskEvent(DatabaseManager.Instance.AuthDatabase.UpdateAccountSessionKey(account, sessionKey.ToHexString()),
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
