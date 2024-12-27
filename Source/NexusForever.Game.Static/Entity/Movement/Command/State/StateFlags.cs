namespace NexusForever.Game.Static.Entity.Movement.Command.State
{
    [Flags]
    public enum StateFlags
    {
        None         = 0x0000,
        Velocity     = 0x0001,
        Move         = 0x0002,
        Jump         = 0x0040,
        Unknown80    = 0x0080,
        Unknown100   = 0x0100,
        Unknown200   = 0x0200,
        Unknown400   = 0x0400,
        DoubleJump   = 0x0800,
        RollForward  = 0x1000,
        RollBackward = 0x2000
    }
}
