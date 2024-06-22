using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Abstract.Housing;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Static.Guild;
using NexusForever.Game.Static.Housing;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Housing
{
    public class ClientHousingCommunityPrivacyLevelHandler : IMessageHandler<IWorldSession, ClientHousingCommunityPrivacyLevel>
    {
        #region Dependency Injection

        private readonly IGlobalResidenceManager globalResidenceManager;

        public ClientHousingCommunityPrivacyLevelHandler(
            IGlobalResidenceManager globalResidenceManager)
        {
            this.globalResidenceManager = globalResidenceManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientHousingCommunityPrivacyLevel housingCommunityPrivacyLevel)
        {
            if (session.Player.Map is not IResidenceMapInstance)
                throw new InvalidPacketValueException();

            // ignore the value in the packet
            ICommunity community = session.Player.GuildManager.GetGuild<ICommunity>(GuildType.Community);
            if (community == null)
                throw new InvalidPacketValueException();

            if (!community.GetMember(session.Player.CharacterId).Rank.HasPermission(GuildRankPermission.ChangeCommunityRemodelOptions))
                throw new InvalidPacketValueException();

            if (housingCommunityPrivacyLevel.PrivacyLevel == CommunityPrivacyLevel.Public)
                globalResidenceManager.RegisterCommunityVisits(community.Residence, community, session.Player.Name);
            else
                globalResidenceManager.DeregisterCommunityVists(community.Residence.Id);

            community.SetCommunityPrivate(housingCommunityPrivacyLevel.PrivacyLevel == CommunityPrivacyLevel.Private);
        }
    }
}
