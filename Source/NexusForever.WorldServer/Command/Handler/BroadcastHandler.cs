using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NexusForever.Shared.Network;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Broadcast")]
    public class BroadcastHandler : NamedCommand
    {
        public BroadcastHandler()
            : base(true, "broadcast", "broadcastTier(0/1/2) message - Broadcast message to the realm using the given broadcast tier.")
        {
        }

        protected override Task HandleCommandAsync(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length > 2)
            {
                List<WorldSession> allSessions = NetworkManager<WorldSession>.GetSessions().ToList();
                foreach(WorldSession session in allSessions)
                    session.EnqueueMessageEncrypted(new ServerRealmBroadcast
                    {
                        Unknown0 = byte.Parse(parameters[0].ToString()),
                        Message = string.Join(" ", parameters, 1, parameters.Length - 1)
                    });
            }

            return Task.CompletedTask;
        }
    }
}
