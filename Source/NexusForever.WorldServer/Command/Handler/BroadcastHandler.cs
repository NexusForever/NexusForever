using System.Collections.Generic;
using System.Threading.Tasks;
using NexusForever.Shared.Network;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Broadcast")]
    public class BroadcastHandler : NamedCommand
    {
        public BroadcastHandler()
            : base(false, "broadcast", "broadcastTier(0/1/2) message - Broadcast message to the realm using the given broadcast tier.")
        {
        }

        protected override Task HandleCommandAsync(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length < 2 || parameters[0].Length > 1)
            {
                context.SendMessageAsync("Parameters are invalid.");
                return Task.CompletedTask;
            }

            BroadcastTier broadcastTier = (BroadcastTier)byte.Parse(parameters[0]);
            if (broadcastTier > BroadcastTier.Low)
            {
                context.SendMessageAsync("Invalid broadcast tier.");
                return Task.CompletedTask;
            }

            IEnumerable<WorldSession> allSessions = NetworkManager<WorldSession>.Instance.GetSessions();
            foreach (WorldSession session in allSessions)
            {
                session.EnqueueMessageEncrypted(new ServerRealmBroadcast
                {
                    Tier = broadcastTier,
                    Message = string.Join(" ", parameters, 1, parameters.Length - 1)
                });
            }

            return Task.CompletedTask;
        }
    }
}
