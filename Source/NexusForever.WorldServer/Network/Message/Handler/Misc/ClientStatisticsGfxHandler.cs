using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Misc
{
    public class ClientStatisticsGfxHandler : IMessageHandler<IWorldSession, ClientStatisticsGfx>
    {
        /// <summary>
        /// Client updates stats counters every 15 seconds and sends counters to server every 120 seconds
        /// Contains statistics about allocated graphics resources though exact use of the fields is unknown.
        /// </summary>
        public void HandleMessage(IWorldSession session, ClientStatisticsGfx gfxStatistics)
        {
        }
    }
}
