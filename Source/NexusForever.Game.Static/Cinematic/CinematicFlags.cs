namespace NexusForever.Game.Static.Cinematic
{
    [Flags]
    public enum CinematicFlags
    {
        ScheduleEnd  = 0x0000,
        EndImmediate = 0x0001,
        Unknown2     = 0x0002,
        NotifyServer = 0x0004,
    }
}
