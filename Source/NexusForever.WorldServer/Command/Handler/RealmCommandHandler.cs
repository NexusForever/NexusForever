using System.Threading.Tasks;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Network;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game.Social;
using NexusForever.WorldServer.Network;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Realm Management")]
    public class RealmCommandHandler : CommandCategory
    {
        public RealmCommandHandler()
            : base(false, "realm", "server")
        {
        }

        [SubCommandHandler("motd", "message - Set the realm's Message of the Day and announce to the realm")]
        public async Task HandleMotd(CommandContext context, string subCommand, string[] parameters)
        {
            if (parameters.Length < 1)
            {
                await SendHelpAsync(context).ConfigureAwait(false);
                return;
            }

            ConfigurationManager<WorldServerConfiguration>.Instance.Config.MessageOfTheDay = string.Join(" ", parameters);
            if (ConfigurationManager<WorldServerConfiguration>.Instance.Save())
            {
                string motd = ConfigurationManager<WorldServerConfiguration>.Instance.Config.MessageOfTheDay;
                foreach (WorldSession session in NetworkManager<WorldSession>.Instance.GetSessions())
                    SocialManager.Instance.SendMessage(session, "MOTD: " + motd, channel: ChatChannel.Realm);

                await context.SendMessageAsync($"MOTD Updated!");
            }
            else
                await context.SendMessageAsync($"MOTD Update Failed!");
        }
    }
}
