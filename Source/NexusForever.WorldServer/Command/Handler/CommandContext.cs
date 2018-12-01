using Microsoft.Extensions.Logging;
using NexusForever.WorldServer.Network;

namespace NexusForever.WorldServer.Command.Handler
{
    public abstract class CommandContext
    {
        protected CommandContext(WorldSession session)
        {
            Session = session;
        }
        public WorldSession Session { get; set; }

        public virtual void SendError(ILogger logger, string text)
        {
            logger.LogWarning(text);
        }
        public virtual void SendMessage(ILogger logger, string text)
        {
            logger.LogInformation(text);
        }
    }
}