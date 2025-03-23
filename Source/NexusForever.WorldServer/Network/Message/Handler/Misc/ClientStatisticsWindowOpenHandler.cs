using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Misc
{
    internal class ClientStatisticsWindowOpenHandler : IMessageHandler<WorldSession, ClientStatisticsWindowOpen>
    {
        /// <summary>
        /// Sent when the client closes an open UI window.
        /// </summary>
        public void HandleMessage(WorldSession session, ClientStatisticsWindowOpen windowOpenStatistics)
        {
        }
    }
}
