using System;
using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Game Master")]
    public class GetCommandHandler : NamedCommand
    {

        public override string HelpText => "get <property>";

        public GetCommandHandler()
            : base(true, "get")
        {
        }

        protected override async Task HandleCommandAsync(CommandContext context, string command, string[] parameters)
        {
            if (parameters.Length != 1)
            {
                await SendHelpAsync(context);
                return;
            }

            if (!Enum.TryParse(parameters[0], out Property property))
            {
                await context.SendErrorAsync($"Unknown property: {parameters[0]}").ConfigureAwait(false);
                return;
            }

            float? value = context.Session.Player.GetPropertyValue(property);
            if (value == null)
                await context.SendMessageAsync($"Property {property} is not set.");
            else
                await context.SendMessageAsync($"{property} = {value}");
        }
    }
}
