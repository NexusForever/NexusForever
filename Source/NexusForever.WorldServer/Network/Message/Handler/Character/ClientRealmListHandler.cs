using System.Linq;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Server;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Static;
using NetworkMessage = NexusForever.Network.Message.Model.Shared.Message;

namespace NexusForever.WorldServer.Network.Message.Handler.Character
{
    public class ClientRealmListHandler : IMessageHandler<IWorldSession, ClientRealmList>
    {
        #region Dependency Injection

        private readonly IRealmContext realmContext;
        private readonly IServerManager serverManager;

        public ClientRealmListHandler(
            IRealmContext realmContext,
            IServerManager serverManager)
        {
            this.realmContext  = realmContext;
            this.serverManager = serverManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientRealmList _)
        {
            var serverRealmList = new ServerRealmList();

            foreach (IServerInfo server in serverManager.Servers)
            {
                RealmStatus status = RealmStatus.Up;
                if (!server.IsOnline && server.Model.Id != realmContext.RealmId)
                    status = RealmStatus.Down;

                serverRealmList.Realms.Add(new ServerRealmList.RealmInfo
                {
                    RealmId          = server.Model.Id,
                    RealmName        = server.Model.Name,
                    Type             = (RealmType)server.Model.Type,
                    Status           = status,
                    Population       = RealmPopulation.Low,
                    Unknown8         = new byte[16],
                    AccountRealmInfo = new ServerRealmList.RealmInfo.AccountRealmData
                    {
                        RealmId = server.Model.Id
                    }
                });
            }

            serverRealmList.Messages = serverManager.ServerMessages
                .Select(m => new NetworkMessage
                {
                    Index    = m.Index,
                    Messages = m.Messages
                })
                .ToList();

            session.EnqueueMessageEncrypted(serverRealmList);
        }
    }
}
