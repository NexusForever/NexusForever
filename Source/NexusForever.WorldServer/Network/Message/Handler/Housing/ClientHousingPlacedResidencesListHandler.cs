using NexusForever.Game;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Character;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Abstract.Housing;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Static.Guild;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Housing;

namespace NexusForever.WorldServer.Network.Message.Handler.Housing
{
    public class ClientHousingPlacedResidencesListHandler : IMessageHandler<IWorldSession, ClientHousingRequestCommunityPlacedResidencesList>
    {
        #region Dependency Injection

        private readonly ICharacterManager characterManager;
        private readonly IRealmContext realmContext;

        public ClientHousingPlacedResidencesListHandler(
            ICharacterManager characterManager,
            IRealmContext realmContext)
        {
            this.characterManager = characterManager;
            this.realmContext     = realmContext;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientHousingRequestCommunityPlacedResidencesList _)
        {
            if (session.Player.Map is not IResidenceMapInstance)
                throw new InvalidPacketValueException();

            ICommunity community = session.Player.GuildManager.GetGuild<ICommunity>(GuildType.Community);
            if (community?.Residence == null)
                throw new InvalidPacketValueException();

            var housingPlacedResidencesList = new ServerHousingPlacedResidencesList();
            foreach (IResidenceChild residenceChild in community.Residence.GetChildren())
            {
                string owner = null;
                if (residenceChild.Residence.OwnerIdentity != null)
                    owner = characterManager.GetCharacter(residenceChild.Residence.OwnerIdentity.Id)?.Name;

                housingPlacedResidencesList.Residences.Add(new ServerHousingPlacedResidencesList.Residence
                {
                    ResidenceIdentity = residenceChild.Residence.Identity.ToNetworkIdentity(),
                    PlayerName    = owner ?? "",
                    PropertyIndex = (uint)residenceChild.Residence.PropertyInfoId - 100
                });
            }

            session.EnqueueMessageEncrypted(housingPlacedResidencesList);
        }
    }
}
