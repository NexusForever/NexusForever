using System.Linq;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Utility;

namespace NexusForever.WorldServer.Network.Message.Handler.Entity.Player
{
    public class ClientInspectPlayerRequestHandler : IMessageHandler<IWorldSession, ClientInspectPlayerRequest>
    {
        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public ClientInspectPlayerRequestHandler(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientInspectPlayerRequest inspectPlayer)
        {
            IPlayer player = playerManager.GetPlayer(inspectPlayer.UnitId);
            if (player == null)
                return;

            session.EnqueueMessageEncrypted(new ServerInspectPlayerResponse
            {
                UnitId = inspectPlayer.UnitId,
                Items = player.Inventory
                    .Single(b => b.Location == InventoryLocation.Equipped)
                    .Select(i => i.Build())
                    .ToList()
            });
        }
    }
}
