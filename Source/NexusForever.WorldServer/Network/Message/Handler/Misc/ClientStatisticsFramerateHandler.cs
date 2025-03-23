using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Misc
{
    internal class ClientStatisticsFramerateHandler : IMessageHandler<WorldSession, ClientStatisticsFramerate>
    {
        /// <summary>
        /// Client waits 120 seconds upon game start and then sends this message every 30 seconds thereafter.
        /// </summary>
        public void HandleMessage(WorldSession session, ClientStatisticsFramerate framerateStatistics)
        {
        }
    }
}
