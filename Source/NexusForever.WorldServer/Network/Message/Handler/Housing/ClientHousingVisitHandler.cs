using System;
using NexusForever.Game;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Abstract.Housing;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Abstract.Map.Lock;
using NexusForever.Game.Map;
using NexusForever.Game.Static.Housing;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Housing;

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
            if (session.Player.Map is not IResidenceMapInstance)
                throw new InvalidPacketValueException();

            if (!session.Player.CanTeleport())
                return;

            IResidence residence;
            if (!string.IsNullOrEmpty(housingVisit.PlayerToVisitName))
                residence = globalResidenceManager.GetResidenceByOwner(housingVisit.PlayerToVisitName);
            else if (!string.IsNullOrEmpty(housingVisit.CommunityToVisitName))
                residence = globalResidenceManager.GetCommunityByOwner(housingVisit.CommunityToVisitName);
            else if (housingVisit.IdentityToVisit != null)
                residence = globalResidenceManager.GetResidenceByOwner(housingVisit.IdentityToVisit.ToGameIdentity());
            else if (housingVisit.CommunityToVisitIdentity != null)
            {
                Identity residenceIdentity = globalGuildManager.GetGuild<ICommunity>(housingVisit.CommunityToVisitIdentity.ToGameIdentity())?.Residence?.Identity ?? null;
                residence = globalResidenceManager.GetResidence(residenceIdentity);
            }
            else
                throw new NotImplementedException();

            if (residence == null)
            {
                //session.Player.SendGenericError();
                // TODO: show error
                return;
            }

            switch (residence.PrivacyLevel)
            {
                case ResidencePrivacyLevel.Private:
                {
                    // TODO: show error
                    return;
                }
                // TODO: check if player is either a neighbour or roommate
                case ResidencePrivacyLevel.NeighboursOnly:
                    break;
                case ResidencePrivacyLevel.RoommatesOnly:
                    break;
            }

            IMapLock mapLock = mapLockManager.GetResidenceLock(residence.Parent ?? residence);

            // teleport player to correct residence instance
            IResidenceEntrance entrance = globalResidenceManager.GetResidenceEntrance(residence.PropertyInfoId);
            session.Player.Rotation = entrance.Rotation.ToEuler();
            session.Player.TeleportTo(new MapPosition
            {
                Info = new MapInfo
                {
                    Entry   = entrance.Entry,
                    MapLock = mapLock
                },
                Position = entrance.Position
            });
        }
    }
}
