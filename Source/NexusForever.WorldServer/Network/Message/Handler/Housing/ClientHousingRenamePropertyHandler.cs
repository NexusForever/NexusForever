using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Abstract.Text.Filter;
using NexusForever.Game.Static.TextFilter;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Housing
{
    public class ClientHousingRenamePropertyHandler
    {
        #region Dependency Injection

        private readonly ITextFilterManager textFilterManager;

        public ClientHousingRenamePropertyHandler(
            ITextFilterManager textFilterManager)
        {
            this.textFilterManager = textFilterManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientHousingRenameProperty housingRenameProperty)
        {
            if (session.Player.Map is not IResidenceMapInstance residenceMap)
                throw new InvalidPacketValueException();

            if (!textFilterManager.IsTextValid(housingRenameProperty.Name)
                || !textFilterManager.IsTextValid(housingRenameProperty.Name, UserText.HousingResidenceName))
                throw new InvalidPacketValueException();

            residenceMap.RenameResidence(session.Player, housingRenameProperty.TargetResidence, housingRenameProperty.Name);
        }
    }
}
