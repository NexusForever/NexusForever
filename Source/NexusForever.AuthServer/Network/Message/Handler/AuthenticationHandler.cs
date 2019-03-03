﻿using System.Linq;
using NexusForever.AuthServer.Network.Message.Model;
using NexusForever.Shared.Cryptography;
using NexusForever.Shared.Database.Auth;
using NexusForever.Shared.Database.Auth.Model;
using NexusForever.Shared.Game;
using NexusForever.Shared.Game.Events;
using NexusForever.Shared.Network.Message;

namespace NexusForever.AuthServer.Network.Message.Handler
{
    public static class AuthenticationHandler
    {
        [MessageHandler(GameMessageOpcode.ClientHelloAuth)]
        public static void HandleHelloAuth(AuthSession session, ClientHelloAuth helloAuth)
        {
            session.EnqueueEvent(new TaskGenericEvent<Account>(AuthDatabase.GetAccountAsync(helloAuth.Email, helloAuth.GameToken.Guid),
                account =>
            {
                if (account == null)
                {
                    // TODO: send error
                    return;
                }

                session.EnqueueMessageEncrypted(new ServerAuthAccepted());
                session.EnqueueMessageEncrypted(new ServerRealmMessages
                {
                    Messages = ServerManager.ServerMessages
                        .Select(m => new ServerRealmMessages.Message
                        {
                            Index    = m.Index,
                            Messages = m.Messages
                        })
                        .ToList()
                });

                byte[] sessionKey = RandomProvider.GetBytes(16u);
                session.EnqueueEvent(new TaskEvent(AuthDatabase.UpdateAccountSessionKey(account, sessionKey),
                    () =>
                {
                    ServerInfo server = ServerManager.Servers.First();
                    session.EnqueueMessageEncrypted(new ServerRealmInfo
                    {
                        AccountId  = account.Id,
                        SessionKey = sessionKey,
                        Realm      = server.Model.Name,
                        Host       = server.Address,
                        Port       = server.Model.Port,
                        Type       = server.Model.Type
                    });
                }));
            }));
        }
    }
}
