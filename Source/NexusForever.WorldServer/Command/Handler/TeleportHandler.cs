using System.Numerics;
using System.Threading.Tasks;
using NexusForever.Shared;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Network;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Teleport")]
    public class TeleportHandler : NamedCommand
    {
        public TeleportHandler()
            : base(true, "teleport", "port")
        {
        }

        private void SendHelpText(CommandContext context)
        {
            string[] helpText = new string[]
                {
                    "Teleport Parameters",
                    "--",
                    "WorldLocation2Id - Teleport to WorldLocation2Id",
                    "X, Y, Z - Teleport to coordinates in current map",
                    "WorldId, X, Y, Z - Teleport to coordinates in target World ID"
                };

            foreach (string line in helpText)
                context.SendMessageAsync(line);
        }

        protected override Task HandleCommandAsync(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length == 0 || parameters.Length == 2 || parameters.Length > 4)
            {
                SendHelpText(context);
                return Task.CompletedTask;
            }
            
            WorldSession session = context.Session;
            if (parameters.Length == 4)
                session.Player.TeleportTo(ushort.Parse(parameters[0]), float.Parse(parameters[1]),
                    float.Parse(parameters[2]), float.Parse(parameters[3]));
            else if (parameters.Length == 3)
                session.Player.TeleportTo((ushort) session.Player.Map.Entry.Id, float.Parse(parameters[0]),
                    float.Parse(parameters[1]), float.Parse(parameters[2]));
            else if (parameters.Length == 1)
            {
                // Assumed teleporting via WorldLocation2 ID
                if (!uint.TryParse(parameters[0], out uint worldLocation2Id))
                {
                    SendHelpText(context);
                    return Task.CompletedTask;
                }
                
                WorldLocation2Entry entry = GameTableManager.WorldLocation2.GetEntry(worldLocation2Id);
                if (entry != null)
                {
                    var rotation = new Quaternion(entry.Facing0, entry.Facing0, entry.Facing2, entry.Facing3);
                    session.Player.Rotation = rotation.ToEulerDegrees();
                    session.Player.TeleportTo((ushort)entry.WorldId, entry.Position0, entry.Position1, entry.Position2);
                }
                else
                    context.SendMessageAsync($"WorldLocation2 Entry not found to match ID: {worldLocation2Id}");
            }

            return Task.CompletedTask;
        }
    }
}
