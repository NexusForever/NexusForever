using System;
using System.Linq;
using NexusForever.Cryptography;
using NexusForever.Database;
using NexusForever.Database.Auth;
using NexusForever.Game;
using NexusForever.Game.Abstract.Server;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Static;
using NexusForever.Shared.Game.Events;

namespace NexusForever.WorldServer.Network.Message.Handler.Character
{
    public class ClientSelectRealmHandler : IMessageHandler<IWorldSession, ClientSelectRealm>
    {
        #region Dependency Injection

        private readonly IServerManager serverManager;
        private readonly IDatabaseManager databaseManager;

        public ClientSelectRealmHandler(
            IServerManager serverManager,
            IDatabaseManager databaseManager)
        {
            this.serverManager   = serverManager;
            this.databaseManager = databaseManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientSelectRealm selectRealm)
        {
            IServerInfo server = serverManager.Servers.SingleOrDefault(s => s.Model.Id == selectRealm.RealmId);
            if (server == null)
                throw new InvalidPacketValueException();

            // clicking back or selecting the current realm also triggers this packet, client crashes if we don't ignore it
            if (server.Model.Id == RealmContext.Instance.RealmId)
                return;

            // TODO: Return proper error packet if server is not online
            if (!server.IsOnline)
            {
                session.EnqueueMessageEncrypted(new ServerForceKick());
                return;
            }

            byte[] sessionKey = RandomProvider.GetBytes(16u);
            session.Events.EnqueueEvent(new TaskEvent(databaseManager.GetDatabase<AuthDatabase>().UpdateAccountSessionKey(session.Account.Id, Convert.ToHexString(sessionKey)),
                () =>
            {
                session.EnqueueMessageEncrypted(new ServerNewRealm
                {
                    SessionKey  = sessionKey,
                    GatewayData = new ServerNewRealm.Gateway
                    {
                        Address = server.Address,
                        Port    = server.Model.Port
                    },
                    RealmName = server.Model.Name,
                    Type      = (RealmType)server.Model.Type
                });
            }));
        }
    }
}
