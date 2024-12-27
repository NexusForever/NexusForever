using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Housing;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Housing
{
    public class ClientHousingRandomResidenceListHandler : IMessageHandler<IWorldSession, ClientHousingRandomResidenceList>
    {
        #region Dependency Injection

        private readonly IGlobalResidenceManager globalResidenceManager;
        private readonly IRealmContext realmContext;

        public ClientHousingRandomResidenceListHandler(
            IGlobalResidenceManager globalResidenceManager,
            IRealmContext realmContext)
        {
            this.globalResidenceManager = globalResidenceManager;
            this.realmContext           = realmContext;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientHousingRandomResidenceList _)
        {
            var serverHousingRandomResidenceList = new ServerHousingRandomResidenceList();
            foreach (IPublicResidence residence in globalResidenceManager.GetRandomVisitableResidences())
            {
                serverHousingRandomResidenceList.Residences.Add(new ServerHousingRandomResidenceList.Residence
                {
                    RealmId     = realmContext.RealmId,
                    ResidenceId = residence.ResidenceId,
                    Owner       = residence.Owner,
                    Name        = residence.Name
                });
            }

            session.EnqueueMessageEncrypted(serverHousingRandomResidenceList);
        }
    }
}
