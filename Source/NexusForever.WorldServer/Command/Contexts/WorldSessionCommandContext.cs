using System;
using System.Threading.Tasks;
using NexusForever.WorldServer.Game.Social;
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

        public override Task SendErrorAsync(string text)
        {
            base.SendErrorAsync(text);
            SendText(text, "Error");
            // TODO: Send player a chat message.
            return Task.CompletedTask;
        }

        private void SendText(string text, string name = "")
        {
            foreach (string line in text.Trim().Split(Environment.NewLine))
                Session.EnqueueMessageEncrypted(new ServerChat
                {
                    Guid = Session.Player.Guid,
                    Channel = ChatChannel.System,
                    Name = name,
                    Text = line
                });
        }

        public override Task SendMessageAsync(string text)
        {
            base.SendMessageAsync(text);
            SendText(text);
            return Task.CompletedTask;
        }
    }
}
