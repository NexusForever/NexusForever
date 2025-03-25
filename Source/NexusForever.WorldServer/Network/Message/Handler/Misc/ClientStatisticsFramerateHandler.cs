using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Misc
{
    public class ClientStatisticsFramerateHandler : IMessageHandler<IWorldSession, ClientStatisticsFramerate>
    {
        /// <summary>
        /// Client waits 120 seconds upon game start and then sends this message every 30 seconds thereafter.
        /// </summary>
        public void HandleMessage(IWorldSession session, ClientStatisticsFramerate framerateStatistics)
        {
        }
    }
}
