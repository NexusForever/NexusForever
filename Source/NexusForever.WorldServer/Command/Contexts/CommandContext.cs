using System.Threading.Tasks;
using NexusForever.Shared.GameTable.Static;
using NexusForever.WorldServer.Network;
using NLog;

namespace NexusForever.WorldServer.Command.Contexts
{
    public abstract class CommandContext
    {
        protected ILogger Logger { get; } = LogManager.GetCurrentClassLogger();
        public WorldSession Session { get; set; }
        public Language Language { get; } = Language.English;

        protected CommandContext(WorldSession session)
        {
            Session = session;
        }

        public virtual Task SendErrorAsync(string text)
        {
            Logger.Warn(text);
            return Task.CompletedTask;
        }

        public virtual Task SendMessageAsync(string text)
        {
            Logger.Info(text);
            return Task.CompletedTask;
        }
    }
}
