using NexusForever.Game;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Housing;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Housing;

namespace NexusForever.WorldServer.Network.Message.Handler.Housing
{
    public class ClientHousingRequestRandomResidenceListHandler : IMessageHandler<IWorldSession, ClientHousingRequestRandomResidenceList>
    {
        #region Dependency Injection

        private readonly IGlobalResidenceManager globalResidenceManager;
        private readonly IRealmContext realmContext;

        public ClientHousingRequestRandomResidenceListHandler(
            IGlobalResidenceManager globalResidenceManager,
            IRealmContext realmContext)
        {
            this.globalResidenceManager = globalResidenceManager;
            this.realmContext           = realmContext;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientHousingRequestRandomResidenceList _)
        {
            var serverHousingRandomResidenceList = new ServerHousingRandomResidenceList();
            foreach (IPublicResidence residence in globalResidenceManager.GetRandomVisitableResidences())
            {
                serverHousingRandomResidenceList.Residences.Add(new ServerHousingRandomResidenceList.Residence
                {
                    ResidenceIdentity = residence.Identity.ToNetworkIdentity(),
                    Owner       = residence.Owner,
                    Name        = residence.Name
                });
            }

            session.EnqueueMessageEncrypted(serverHousingRandomResidenceList);
        }
    }
}
