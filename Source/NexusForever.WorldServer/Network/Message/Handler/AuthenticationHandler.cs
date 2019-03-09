﻿using NexusForever.Shared.Database.Auth;
using NexusForever.Shared.Database.Auth.Model;
using NexusForever.Shared.Game.Events;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class AuthenticationHandler
    {
        [MessageHandler(GameMessageOpcode.ClientHelloRealm)]
        public static void HandleHelloRealm(WorldSession session, ClientHelloRealm helloRealm)
        {
            // prevent packets from being processed until asynchronous account select task is complete
            session.CanProcessPackets = false;

            session.EnqueueEvent(new TaskGenericEvent<Account>(AuthDatabase.GetAccountAsync(helloRealm.Email, helloRealm.SessionKey),
                account =>
            {
                if (account == null)
                    throw new InvalidPacketValueException($"Failed to find account, Id:{helloRealm.AccountId}, Email:{helloRealm.Email}, SessionKey:{helloRealm.SessionKey}!");

                session.Initialise(account);
                session.SetEncryptionKey(helloRealm.SessionKey);
                session.CanProcessPackets = true;
            }));
        }
    }
}
