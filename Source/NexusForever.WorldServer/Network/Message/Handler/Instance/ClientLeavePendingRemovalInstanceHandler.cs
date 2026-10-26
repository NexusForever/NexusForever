using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Instance;

namespace NexusForever.WorldServer.Network.Message.Handler.Instance
{
    /// <summary>
    /// Sent from the Leave button of the pending removal warning, the player leaves now instead of when the timer runs out.
    /// </summary>
    public class ClientLeavePendingRemovalInstanceHandler : IMessageHandler<IWorldSession, ClientLeavePendingRemovalInstance>
    {
        public void HandleMessage(IWorldSession session, ClientLeavePendingRemovalInstance leavePendingRemovalInstance)
        {
            if (session.Player.Map is IMapInstance instance)
                instance.LeavePendingRemoval(session.Player);
        }
    }
}
