using NexusForever.WorldServer.Command.Attributes;

namespace NexusForever.WorldServer.Command.Handler
{
    [Name("Game Master")]
    public class JumpHeightCommandHandler : PlayerPropertyChangeCommand
    {

        public override string HelpText =>
            "jumpheight <value> - Set movement speed multiplier. Higher numbers are faster.";

        public JumpHeightCommandHandler()
            : base(p => p.JumpHeight, "jumpheight")
        {
        }
    }
}
