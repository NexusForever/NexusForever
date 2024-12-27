using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.WorldServer.Network.Message.Handler.Vendor
{
    public class ClientBuybackItemFromVendorHandler : IMessageHandler<IWorldSession, ClientBuybackItemFromVendor>
    {
        #region Dependency Injection

        private readonly IBuybackManager buybackManager;

        public ClientBuybackItemFromVendorHandler(
            IBuybackManager buybackManager)
        {
            this.buybackManager = buybackManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientBuybackItemFromVendor buybackItemFromVendor)
        {
            IBuybackItem buybackItem = buybackManager.GetItem(session.Player, buybackItemFromVendor.UniqueId);
            if (buybackItem == null)
                return;

            //TODO Ensure player has room in inventory
            if (session.Player.Inventory.GetInventorySlotsRemaining(InventoryLocation.Inventory) < 1)
            {
                session.Player.SendGenericError(GenericError.ItemInventoryFull);
                return;
            }

            // do all sanity checks before modifying currency
            foreach ((CurrencyType currencyTypeId, ulong currencyAmount) in buybackItem.CurrencyChange)
                if (!session.Player.CurrencyManager.CanAfford(currencyTypeId, currencyAmount))
                    return;

            foreach ((CurrencyType currencyTypeId, ulong currencyAmount) in buybackItem.CurrencyChange)
                session.Player.CurrencyManager.CurrencySubtractAmount(currencyTypeId, currencyAmount);

            session.Player.Inventory.AddItem(buybackItem.Item, InventoryLocation.Inventory);
            buybackManager.RemoveItem(session.Player, buybackItem);
        }
    }
}
