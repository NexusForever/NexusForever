using Microsoft.Extensions.Logging;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using System;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Receive")]
    public class PacketHandler : NamedCommand
    {
        public PacketHandler(ILogger<PacketHandler> logger)
            : base(new[] { "receive" }, true, logger)
        {
        }

        protected override void HandleCommand(CommandContext context, string command, string[] parameters)
        {
            foreach (string packet in parameters)
            {
                string[] split = packet.Split(':');
                context.Session.EnqueueMessageEncrypted((uint)Int32.Parse(split[0]), split[1]);
            }
        }
    }
}
