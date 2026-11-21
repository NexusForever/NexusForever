namespace NexusForever.Game.Static.Cinematic
{
    [Flags]
    public enum CameraAddFlags
    {
        WhiteOut                  = 0x0,
        AddCamera                 = 0x1,
        NotifyCancelNextCamera    = 0x4,
        NotifyCancelCurrentCamera = 0x8
    }
}
