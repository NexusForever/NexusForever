using System.Threading.Tasks;
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

        public virtual Task SendErrorAsync(ILogger logger, string text)
        {
            logger.LogWarning(text);
            return Task.CompletedTask;
        }

        public virtual Task SendMessageAsync(ILogger logger, string text)
        {
            logger.LogInformation(text);
            return Task.CompletedTask;
        }
    }
}
