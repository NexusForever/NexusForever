using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Misc
{
    public class ClientStatisticsWindowOpenHandler : IMessageHandler<IWorldSession, ClientStatisticsWindowOpen>
    {
        /// <summary>
        /// Sent when the client closes an open UI window.
        /// </summary>
        public void HandleMessage(IWorldSession session, ClientStatisticsWindowOpen windowOpenStatistics)
        {
        }
    }
}
