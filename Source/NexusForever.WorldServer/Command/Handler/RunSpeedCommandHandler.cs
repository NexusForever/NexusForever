using NexusForever.WorldServer.Command.Attributes;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Game Master")]
    public class RunSpeedCommandHandler : PlayerPropertyChangeCommand
    {

        public override string HelpText =>
            "runspeed <value> - Set movement speed multiplier. Higher numbers are faster.";

        public RunSpeedCommandHandler()
            : base(p => p.MoveSpeedMultiplier, "runspeed", "speed")
        {
        }
    }
}
