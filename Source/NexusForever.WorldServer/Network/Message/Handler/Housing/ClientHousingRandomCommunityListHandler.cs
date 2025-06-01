using NexusForever.Game;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Housing;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Housing;

namespace NexusForever.WorldServer.Network.Message.Handler.Housing
{
    public class ClientHousingRandomCommunityListHandler : IMessageHandler<IWorldSession, ClientHousingRequestRandomCommunityList>
    {
        #region Dependency Injection

        private readonly IGlobalResidenceManager globalResidenceManager;
        private readonly IRealmContext realmContext;

        public ClientHousingRandomCommunityListHandler(
            IGlobalResidenceManager globalResidenceManager,
            IRealmContext realmContext)
        {
            this.globalResidenceManager = globalResidenceManager;
            this.realmContext           = realmContext;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientHousingRequestRandomCommunityList _)
        {
            var serverHousingRandomCommunityList = new ServerHousingRandomCommunityList();
            foreach (IPublicCommunity community in globalResidenceManager.GetRandomVisitableCommunities())
            {
                serverHousingRandomCommunityList.Communities.Add(new ServerHousingRandomCommunityList.Community
                {
                    GuildIdentity   = community.GuildIdentity.ToNetworkIdentity(),
                    CommunityLeader = community.Owner,
                    Name            = community.Name
                });
            }

            session.EnqueueMessageEncrypted(serverHousingRandomCommunityList);
        }
    }
}
