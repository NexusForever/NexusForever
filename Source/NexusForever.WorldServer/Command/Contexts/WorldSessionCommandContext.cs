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

        public override void SendError(ILogger logger, string text)
        {
            base.SendError(logger, text);
            // TODO: Send player a chat message.
        }

        public override void SendMessage(ILogger logger, string text)
        {
            base.SendMessage(logger, text);
            // TODO: Send player a chat message.
        }
    }
}
