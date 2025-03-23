using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Misc
{
    internal class ClientStatisticsConnectionHandler : IMessageHandler<WorldSession, ClientStatisticsConnection>
    {
        /// <summary>
        /// Client sends this every 60 seconds.
        /// </summary>
        public void HandleMessage(WorldSession session, ClientStatisticsConnection connectionStatistics)
        {
        }
    }
}
