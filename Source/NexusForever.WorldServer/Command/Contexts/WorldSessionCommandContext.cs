using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NexusForever.WorldServer.Network;

namespace NexusForever.WorldServer.Command.Contexts
{
    public class WorldSessionCommandContext : CommandContext
    {
        public WorldSessionCommandContext(WorldSession session)
            : base(session)
        {
        }

        public override Task SendErrorAsync(ILogger logger, string text)
        {
            base.SendErrorAsync(logger, text);
            // TODO: Send player a chat message.
            return Task.CompletedTask;
        }

        public override Task SendMessageAsync(ILogger logger, string text)
        {
            base.SendMessageAsync(logger, text);
            // TODO: Send player a chat message.
            return Task.CompletedTask;
        }
    }
}
