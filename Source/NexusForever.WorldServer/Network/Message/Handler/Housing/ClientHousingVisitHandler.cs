using NexusForever.Game;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Abstract.Housing;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Abstract.Map.Lock;
using NexusForever.Game.Housing;
using NexusForever.Game.Map;
using NexusForever.Game.Static.Guild;
using NexusForever.Game.Static.Housing;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Housing;
using NetworkIdentity = NexusForever.Network.World.Message.Model.Shared.Identity;

namespace NexusForever.WorldServer.Network.Message.Handler.Housing
{
    public class ClientHousingVisitHandler : IMessageHandler<IWorldSession, ClientHousingVisit>
    {
        #region Dependency Injection

        private readonly IGlobalResidenceManager globalResidenceManager;
        private readonly IGlobalGuildManager globalGuildManager;
        private readonly IMapLockManager mapLockManager;

        public ClientHousingVisitHandler(
            IGlobalResidenceManager globalResidenceManager,
            IGlobalGuildManager globalGuildManager,
            IMapLockManager mapLockManager)
        {
            this.globalResidenceManager = globalResidenceManager;
            this.globalGuildManager     = globalGuildManager;
            this.mapLockManager         = mapLockManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientHousingVisit housingVisit)
        {
            IPlayer player = session.Player;
            if (player.Map is not IResidenceMapInstance)
                throw new InvalidPacketValueException();

            if (!player.CanTeleport())
                return;

            IResidence residence = FindResidence(housingVisit);
            if (residence == null)
            {
                SendVisitResult(session, HousingResult.Visit_Failed);
                return;
            }

            // The server does not yet record neighbour or roommate relationships.
            if (residence.Type != ResidenceType.Community && residence.OwnerIdentity != player.Identity
                && residence.PrivacyLevel != ResidencePrivacyLevel.Public)
            {
                SendVisitResult(session, HousingResult.Visit_Private, residence);
                return;
            }

            IResidence mapResidence = residence.Parent ?? residence;
            if (mapResidence.Type == ResidenceType.Community)
            {
                var guildIdentity = mapResidence.GuildOwnerIdentity;
                if (guildIdentity == null || globalGuildManager.GetGuild(guildIdentity.Id) is not ICommunity community
                    || community.Identity != guildIdentity || community.Residence?.Identity != mapResidence.Identity)
                {
                    SendVisitResult(session, HousingResult.Visit_Failed, residence);
                    return;
                }

                if (community.HasFlags(GuildFlag.CommunityPrivate)
                    && (player.Identity.RealmId != guildIdentity.RealmId || community.GetMember(player.CharacterId) == null))
                {
                    SendVisitResult(session, HousingResult.Visit_Private, residence);
                    return;
                }
            }

            IResidenceEntrance entrance;
            try
            {
                entrance = globalResidenceManager.GetResidenceEntrance(residence.PropertyInfoId);
            }
            catch (HousingException)
            {
                SendVisitResult(session, HousingResult.Visit_InvalidWorld, residence);
                return;
            }

            if (entrance?.Entry == null)
            {
                SendVisitResult(session, HousingResult.Visit_InvalidWorld, residence);
                return;
            }

            IMapLock mapLock = mapLockManager.GetResidenceLock(mapResidence);
            player.Rotation = entrance.Rotation.ToEuler();
            player.TeleportTo(new MapPosition
            {
                Info = new MapInfo
                {
                    Entry   = entrance.Entry,
                    MapLock = mapLock
                },
                Position = entrance.Position
            });
        }

        private IResidence FindResidence(ClientHousingVisit housingVisit)
        {
            if (!string.IsNullOrEmpty(housingVisit.PlayerToVisitName))
                return globalResidenceManager.GetResidenceByOwner(housingVisit.PlayerToVisitName);

            if (!string.IsNullOrEmpty(housingVisit.CommunityToVisitName))
                return globalResidenceManager.GetCommunityByOwner(housingVisit.CommunityToVisitName);

            if (housingVisit.IdentityToVisit is { Id: > 0 })
                return globalResidenceManager.GetResidenceByOwner(housingVisit.IdentityToVisit.ToGameIdentity());

            if (housingVisit.CommunityToVisitIdentity is { Id: > 0 })
                return globalResidenceManager.GetCommunityByOwner(housingVisit.CommunityToVisitIdentity.ToGameIdentity());

            return null;
        }

        private static void SendVisitResult(IWorldSession session, HousingResult result, IResidence residence = null)
        {
            session.EnqueueMessageEncrypted(new ServerHousingResult
            {
                ResidenceIdentity = residence?.Identity?.ToNetworkIdentity() ?? new NetworkIdentity(),
                PlayerName        = session.Player.Name,
                Result            = result
            });
        }
    }
}
