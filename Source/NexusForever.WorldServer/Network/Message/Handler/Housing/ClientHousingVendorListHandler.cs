using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Housing
{
    public class ClientHousingVendorListHandler : IMessageHandler<IWorldSession, ClientHousingVendorList>
    {
        #region Dependency Injection

        private readonly IGameTableManager gameTableManager;

        public ClientHousingVendorListHandler(
            IGameTableManager gameTableManager)
        {
            this.gameTableManager = gameTableManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientHousingVendorList _)
        {
            var serverHousingVendorList = new ServerHousingVendorList
            {
                ListType = 0
            };

            // TODO: this isn't entirely correct
            foreach (HousingPlugItemEntry entry in gameTableManager.HousingPlugItem.Entries)
            {
                serverHousingVendorList.PlugItems.Add(new ServerHousingVendorList.PlugItem
                {
                    PlugItemId = entry.Id
                });
            }

            session.EnqueueMessageEncrypted(serverHousingVendorList);
        }
    }
}
