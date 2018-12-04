using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NexusForever.Shared.Database.Auth.Model;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;

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
            Session.EnqueueMessage(new ServerChat()
            {
                Channel = Game.Social.ChatChannel.System,
                GM = true,
                Text = text
            });
            // TODO: Send player a chat message.
            return Task.CompletedTask;
        }

        public override Task SendMessageAsync(ILogger logger, string text)
        {
            base.SendMessageAsync(logger, text);
            Session.EnqueueMessage(new ServerChat()
            {
                Channel = Game.Social.ChatChannel.System,
                GM = true,
                Text = "Error: " + text
            });
            return Task.CompletedTask;
        }
    }
}
