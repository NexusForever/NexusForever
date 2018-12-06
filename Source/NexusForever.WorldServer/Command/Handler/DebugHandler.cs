using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NexusForever.Shared.Network;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Debug")]
    public class DebugHandler : CommandCategory
    {
        public DebugHandler()
            : base(true, "debug")
        {
        }

        [SubCommandHandler("packet", "packet opcode [bytes in hex] - Send debug packet")]
        public async Task HandleDebugPacket(CommandContext context, string subCommand, string[] parameters)
        {
            if (parameters.Length < 1)
                return;

            using (MemoryStream stream = new MemoryStream())
            using (GamePacketWriter writer = new GamePacketWriter(stream))
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (i == 0)
                        writer.Write(Convert.ToUInt64(parameters[i], 16), 16);
                    else
                    {
                        string[] parts = parameters[i].Split("|");
                        ulong value;

                        if (Regex.IsMatch(parts[0], "^[0-9]+$"))
                            value = Convert.ToUInt64(parts[0], 10);
                        else if (Regex.IsMatch(parts[0], "^0b[0-1]+$"))
                            value = Convert.ToUInt64(parts[0].Substring(2), 2);
                        else if (Regex.IsMatch(parts[0], "^0x[0-9A-F]+$"))
                            value = Convert.ToUInt64(parts[0].Substring(2), 16);
                        else if (parts[0] == "PLAYER")
                            value = context.Session.Player.Guid;
                        else
                            continue;

                        writer.Write(value, parts.Length > 1 ? Convert.ToUInt32(parts[1], 10) : 64);
                    }
                }

                writer.FlushBits();

                byte[] data      = stream.ToArray();
                byte[] encrypted = context.Session.Encryption.Encrypt(data, data.Length);

                context.Session.EnqueueMessage(context.Session.BuildEncryptedMessage(encrypted));
            }
        }
    }
}
