using System;
using NexusForever.Database;
using NexusForever.Database.Auth;
using NexusForever.Database.Auth.Model;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared.Game.Events;

namespace NexusForever.WorldServer.Network.Message.Handler.Authentication
{
    public class ClientHelloRealmHandler : IMessageHandler<IWorldSession, ClientHelloRealm>
    {
        #region Depenency Injection

        private readonly IDatabaseManager databaseManager;

        public ClientHelloRealmHandler(
            IDatabaseManager databaseManager)
        {
            this.databaseManager = databaseManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientHelloRealm helloRealm)
        {
            // prevent packets from being processed until asynchronous account select task is complete
            session.CanProcessIncomingPackets = false;

            string sessionKey = Convert.ToHexString(helloRealm.SessionKey);
            session.Events.EnqueueEvent(new TaskGenericEvent<AccountModel>(databaseManager.GetDatabase<AuthDatabase>().GetAccountBySessionKeyAsync(helloRealm.Email, sessionKey),
                account =>
            {
                if (account == null)
                    throw new InvalidPacketValueException($"Failed to find account, Id:{helloRealm.AccountId}, Email:{helloRealm.Email}, SessionKey:{sessionKey}!");

                session.Initialise(account);
                session.SetEncryptionKey(helloRealm.SessionKey);
                session.CanProcessIncomingPackets = true;
            }));
        }
    }
}
