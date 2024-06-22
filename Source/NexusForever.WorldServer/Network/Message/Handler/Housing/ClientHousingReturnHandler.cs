using NexusForever.Game;
using NexusForever.Game.Abstract.Housing;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Map;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Housing
{
    public class ClientHousingReturnHandler : IMessageHandler<IWorldSession, ClientHousingReturn>
    {
        #region Dependency Injection

        private readonly IGlobalResidenceManager globalResidenceManager;

        public ClientHousingReturnHandler(
            IGlobalResidenceManager globalResidenceManager)
        {
            this.globalResidenceManager = globalResidenceManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientHousingReturn _)
        {
            // housing return button will only be visible on other residence maps
            IResidence residence = session.Player.ResidenceManager.Residence;
            if (session.Player.Map is not IResidenceMapInstance
                || session.Player.Map == residence?.Map)
                throw new InvalidPacketValueException();

            // return player to correct residence instance
            IResidenceEntrance entrance = globalResidenceManager.GetResidenceEntrance(residence.PropertyInfoId);
            session.Player.Rotation = entrance.Rotation.ToEulerDegrees();
            session.Player.TeleportTo(new MapPosition
            {
                Info = new MapInfo
                {
                    Entry      = entrance.Entry,
                    InstanceId = residence.Parent?.Id ?? residence.Id
                },
                Position = entrance.Position
            });
        }
    }
}
