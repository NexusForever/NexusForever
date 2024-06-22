using NexusForever.Game;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Abstract.Housing;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Map;
using NexusForever.Game.Static.Guild;
using NexusForever.Game.Static.Housing;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Housing
{
    public class ClientHousingCommunityPlacementHandler : IMessageHandler<IWorldSession, ClientHousingCommunityPlacement>
    {
        #region Dependency Injection

        private readonly IGlobalResidenceManager globalResidenceManager;

        public ClientHousingCommunityPlacementHandler(
            IGlobalResidenceManager globalResidenceManager)
        {
            this.globalResidenceManager = globalResidenceManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientHousingCommunityPlacement housingCommunityPlacement)
        {
            if (session.Player.Map is not IResidenceMapInstance)
                throw new InvalidPacketValueException();

            ICommunity community = session.Player.GuildManager.GetGuild<ICommunity>(GuildType.Community);
            if (community?.Residence == null)
                throw new InvalidPacketValueException();

            IResidenceEntrance entrance = globalResidenceManager.GetResidenceEntrance((PropertyInfoId)(housingCommunityPlacement.PropertyIndex + 100));
            if (entrance == null)
                throw new InvalidPacketValueException();

            IResidence residence = session.Player.ResidenceManager.Residence;
            if (residence == null)
                throw new InvalidPacketValueException();

            if (residence.Parent != null)
            {
                if (community.Residence.GetChild(session.Player.CharacterId) == null)
                    throw new InvalidPacketValueException();

                // for residences on a community just remove the residence
                // any players on the map at the time can stay in the instance
                if (residence.Map != null)
                    residence.Map.RemoveChild(residence);
                else
                    residence.Parent.RemoveChild(residence);

                session.Player.Rotation = entrance.Rotation.ToEulerDegrees();
                session.Player.TeleportTo(entrance.Entry, entrance.Position, community.Residence.Id);
            }
            else
            {
                // move owner to new instance only if not on the same instance as the residence
                // otherwise they will be moved to the new instance during the unload
                if (residence.Map != session.Player.Map)
                {
                    session.Player.Rotation = entrance.Rotation.ToEulerDegrees();
                    session.Player.TeleportTo(entrance.Entry, entrance.Position, community.Residence.Id);
                }

                // for individual residences remove the entire instance
                // move any players on the map at the time to the community
                residence.Map?.Unload(new MapPosition
                {
                    Info = new MapInfo
                    {
                        Entry      = entrance.Entry,
                        InstanceId = community.Residence.Id,
                    },
                    Position = entrance.Position
                });
            }

            // update residence with new plot location and add to community
            residence.PropertyInfoId = (PropertyInfoId)(housingCommunityPlacement.PropertyIndex + 100);

            if (community.Residence.Map != null)
                community.Residence.Map.AddChild(residence, true);
            else
                community.Residence.AddChild(residence, true);
        }
    }
}
