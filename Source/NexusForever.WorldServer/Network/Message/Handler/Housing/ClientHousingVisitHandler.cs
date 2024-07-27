using System;
using NexusForever.Game;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Abstract.Housing;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Abstract.Map.Lock;
using NexusForever.Game.Map;
using NexusForever.Game.Static.Housing;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

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
            if (!string.IsNullOrEmpty(housingVisit.TargetResidenceName))
                residence = globalResidenceManager.GetResidenceByOwner(housingVisit.TargetResidenceName);
            else if (!string.IsNullOrEmpty(housingVisit.TargetCommunityName))
                residence = globalResidenceManager.GetCommunityByOwner(housingVisit.TargetCommunityName);
            else if (housingVisit.TargetResidence.ResidenceId != 0ul)
                residence = globalResidenceManager.GetResidence(housingVisit.TargetResidence.ResidenceId);
            else if (housingVisit.TargetCommunity.NeighbourhoodId != 0ul)
            {
                ulong residenceId = globalGuildManager.GetGuild<ICommunity>(housingVisit.TargetCommunity.NeighbourhoodId)?.Residence?.Id ?? 0ul;
                residence = globalResidenceManager.GetResidence(residenceId);
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
                case ResidencePrivacyLevel.NeighborsOnly:
                    break;
                case ResidencePrivacyLevel.RoommatesOnly:
                    break;
            }

            IMapLock mapLock = mapLockManager.GetResidenceLock(residence.Parent ?? residence);

            // teleport player to correct residence instance
            IResidenceEntrance entrance = globalResidenceManager.GetResidenceEntrance(residence.PropertyInfoId);
            session.Player.Rotation = entrance.Rotation.ToEulerDegrees();
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
