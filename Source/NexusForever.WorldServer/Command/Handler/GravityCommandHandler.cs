using NexusForever.WorldServer.Command.Attributes;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Game Master")]
    public class GravityCommandHandler : PlayerPropertyChangeCommand
    {

        public override string HelpText => "gravity <value> - Set gravity multiplier";

        public GravityCommandHandler()
            : base(p => p.GravityMultiplier, "gravity")
        {
        }
    }
}
