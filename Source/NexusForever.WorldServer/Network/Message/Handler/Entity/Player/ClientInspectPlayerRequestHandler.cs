using System.Linq;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

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
            IPlayer player = playerManager.GetPlayer(inspectPlayer.Guid);
            if (player == null)
                return;

            session.EnqueueMessageEncrypted(new ServerInspectPlayerResponse
            {
                Guid = inspectPlayer.Guid,
                Items = player.Inventory
                    .Single(b => b.Location == InventoryLocation.Equipped)
                    .Select(i => i.Build())
                    .ToList()
            });
        }
    }
}
