using System;
using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Game Master")]
    public class SetCommandHandler : NamedCommand
    {

        public override string HelpText => "set <property> <value>";

        public SetCommandHandler()
            : base(true, "set")
        {
        }

        protected override async Task HandleCommandAsync(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length != 2)
            {
                await SendHelpAsync(context);
                return;
            }

            if (!Enum.TryParse(parameters[0], out Property property))
            {
                await context.SendErrorAsync($"Unknown property: {parameters[0]}").ConfigureAwait(false);
                return;
            }

            if (!float.TryParse(parameters[1], out float propertyValue))
            {
                await context.SendErrorAsync($"Invalid property value: {parameters[1]}").ConfigureAwait(false);
                return;
            }

            context.Session.Player.SetProperty(property, propertyValue);
            context.Session.Player.TeleportTo((ushort) context.Session.Player.Map.Entry.Id,
                context.Session.Player.Position.X, context.Session.Player.Position.Y,
                context.Session.Player.Position.Z);
            await context.SendMessageAsync($"Set {property} to {propertyValue}. Teleporting in place to force update.");
        }
    }
}
