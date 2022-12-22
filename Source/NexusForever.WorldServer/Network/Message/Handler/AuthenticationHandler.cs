using NexusForever.Database.Auth;
using NexusForever.Database;
using NexusForever.Database.Auth.Model;
using NexusForever.Game.Network;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared.Game.Events;
using System;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class AuthenticationHandler
    {
        [MessageHandler(GameMessageOpcode.ClientHelloRealm)]
        public static void HandleHelloRealm(WorldSession session, ClientHelloRealm helloRealm)
        {
            // prevent packets from being processed until asynchronous account select task is complete
            session.CanProcessPackets = false;

            string sessionKey = Convert.ToHexString(helloRealm.SessionKey);
            session.Events.EnqueueEvent(new TaskGenericEvent<AccountModel>(DatabaseManager.Instance.GetDatabase<AuthDatabase>().GetAccountBySessionKeyAsync(helloRealm.Email, sessionKey),
                account =>
            {
                if (account == null)
                    throw new InvalidPacketValueException($"Failed to find account, Id:{helloRealm.AccountId}, Email:{helloRealm.Email}, SessionKey:{sessionKey}!");

                session.Initialise(account);
                session.SetEncryptionKey(helloRealm.SessionKey);
                session.CanProcessPackets = true;
            }));
        }
    }
}
