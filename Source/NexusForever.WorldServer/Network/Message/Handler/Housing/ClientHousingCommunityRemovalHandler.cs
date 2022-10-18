using System;
using NexusForever.Game;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Abstract.Housing;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Abstract.Map.Lock;
using NexusForever.Game.Static.Guild;
using NexusForever.Game.Static.Housing;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Housing
{
    public class ClientHousingCommunityRemovalHandler : IMessageHandler<IWorldSession, ClientHousingCommunityRemoval>
    {
        #region Dependency Injection

        private readonly IGlobalResidenceManager globalResidenceManager;
        private readonly IMapLockManager mapLockManager;

        public ClientHousingCommunityRemovalHandler(
            IGlobalResidenceManager globalResidenceManager,
            IMapLockManager mapLockManager)
        {
            this.globalResidenceManager = globalResidenceManager;
            this.mapLockManager         = mapLockManager;
        }

        #endregion

        // TODO: investigate why this doesn't get triggered on another housing plot
        // client has a global variable that is only set when receiving hosuing plots which isn't set when on another housing plot
        public void HandleMessage(IWorldSession session, ClientHousingCommunityRemoval housingCommunityRemoval)
        {
            if (session.Player.Map is not IResidenceMapInstance)
                throw new InvalidPacketValueException();

            ICommunity community = session.Player.GuildManager.GetGuild<ICommunity>(GuildType.Community);
            if (community?.Residence == null)
                throw new InvalidPacketValueException();

            IResidenceEntrance entrance = globalResidenceManager.GetResidenceEntrance(PropertyInfoId.Residence);
            if (entrance == null)
                throw new InvalidOperationException();

            IResidenceChild child = community.Residence.GetChild(session.Player.CharacterId);
            if (child == null)
                throw new InvalidOperationException();

            if (child.Residence.Map != null)
                child.Residence.Map.RemoveChild(child.Residence);
            else
                child.Residence.Parent.RemoveChild(child.Residence);

            child.Residence.PropertyInfoId = PropertyInfoId.Residence;

            IMapLock mapLock = mapLockManager.GetResidenceLock(child.Residence);

            // shouldn't need to check for existing instance
            // individual residence instances are unloaded when transfered to a community
            // if for some reason the instance is still unloading the residence will be initalised again after
            session.Player.Rotation = entrance.Rotation.ToEuler();
            session.Player.TeleportTo(entrance.Entry, entrance.Position, mapLock);
        }
    }
}
