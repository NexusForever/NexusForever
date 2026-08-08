using NexusForever.Game.Abstract.Entity;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Housing;

namespace NexusForever.WorldServer.Network.Message.Handler.Item
{
    public class ClientHousingAddItemToCrateHandler : IMessageHandler<IWorldSession, ClientHousingAddItemToCrate>
    {
        #region Dependency Injection

        private readonly IGameTableManager gameTableManager;

        public ClientHousingAddItemToCrateHandler(
            IGameTableManager gameTableManager)
        {
            this.gameTableManager = gameTableManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientHousingAddItemToCrate addItemToCrate)
        {
            IItem item = session.Player.Inventory.GetItem(addItemToCrate.ItemGuid);
            if (item == null)
                throw new InvalidPacketValueException();

            HousingDecorInfoEntry entry = gameTableManager.HousingDecorInfo.GetEntry(item.Info.Entry.HousingDecorInfoId);
            if (entry == null)
                throw new InvalidPacketValueException();

            if (session.Player.Inventory.ItemUse(item))
                session.Player.ResidenceManager.DecorCreate(entry);
        }
    }
}
