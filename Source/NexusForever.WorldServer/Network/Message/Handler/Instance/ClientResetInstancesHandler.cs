using System.Linq;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Abstract.Map.Lock;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Instance;

namespace NexusForever.WorldServer.Network.Message.Handler.Instance
{
    /// <summary>
    /// The client's /resetinstances: drops the player's solo instances, the next entry to one of those worlds creates a
    /// new instance. The instance the player is in stays.
    /// </summary>
    /// <remarks>
    /// The result is a system message: ServerInstanceResetResult fires the client's OnInstanceResetResult, which only the
    /// Instance Settings dialog handles (its own reset button); without that dialog open, the InstanceSettings addon
    /// errors on it.
    /// TODO: group instances (reset by the group leader when nobody is inside).
    /// </remarks>
    public class ClientResetInstancesHandler : IMessageHandler<IWorldSession, ClientResetInstances>
    {
        #region Dependency Injection

        private readonly IMapLockManager mapLockManager;

        public ClientResetInstancesHandler(
            IMapLockManager mapLockManager)
        {
            this.mapLockManager = mapLockManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientResetInstances resetInstances)
        {
            var current = (session.Player.Map as IMapInstance)?.MapLock;

            var toReset = mapLockManager.GetSoloLocks(session.Player.Identity)
                .Where(l => l.InstanceId != current?.InstanceId)
                .ToList();
            foreach (IMapLock mapLock in toReset)
                mapLockManager.RemoveSoloLock(session.Player.Identity, mapLock.WorldId);

            if (toReset.Count == 0)
                session.Player.SendSystemMessage("You have no instances to reset.");
            else if (current != null)
                session.Player.SendSystemMessage("Your instances have been reset. The instance you are in was kept.");
            else
                session.Player.SendSystemMessage("Your instances have been reset.");
        }
    }
}
