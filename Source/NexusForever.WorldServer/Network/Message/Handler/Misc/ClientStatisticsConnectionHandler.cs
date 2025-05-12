using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Misc
{
    public class ClientStatisticsConnectionHandler : IMessageHandler<IWorldSession, ClientStatisticsConnection>
    {
        /// <summary>
        /// Client sends this every 60 seconds.
        /// </summary>
        public void HandleMessage(IWorldSession session, ClientStatisticsConnection connectionStatistics)
        {
        }
    }
}
