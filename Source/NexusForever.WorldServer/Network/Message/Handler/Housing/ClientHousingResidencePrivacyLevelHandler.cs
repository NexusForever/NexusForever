using NexusForever.Game.Abstract.Housing;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Static.Housing;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Housing
{
    public class ClientHousingResidencePrivacyLevelHandler : IMessageHandler<IWorldSession, ClientHousingSetPrivacyLevel>
    {
        #region Dependency Injection

        private readonly IGlobalResidenceManager globalResidenceManager;

        public ClientHousingResidencePrivacyLevelHandler(
            IGlobalResidenceManager globalResidenceManager)
        {
            this.globalResidenceManager = globalResidenceManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientHousingSetPrivacyLevel housingSetPrivacyLevel)
        {
            if (session.Player.Map is not IResidenceMapInstance)
                throw new InvalidPacketValueException();

            if (session.Player.ResidenceManager.Residence == null)
                throw new InvalidPacketValueException();

            if (housingSetPrivacyLevel.PrivacyLevel == ResidencePrivacyLevel.Public)
                globalResidenceManager.RegisterResidenceVists(session.Player.ResidenceManager.Residence, session.Player.Name);
            else
                globalResidenceManager.DeregisterResidenceVists(session.Player.ResidenceManager.Residence.Id);

            session.Player.ResidenceManager.SetResidencePrivacy(housingSetPrivacyLevel.PrivacyLevel);
        }
    }
}
