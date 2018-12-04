using Microsoft.Extensions.Logging;
using NexusForever.WorldServer.Network;

namespace NexusForever.WorldServer.Command.Contexts
{
    public abstract class CommandContext
    {
        public WorldSession Session { get; set; }

        protected CommandContext(WorldSession session)
        {
            Session = session;
        }

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
