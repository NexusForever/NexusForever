namespace NexusForever.Game.Static.Matching
{
    [Flags]
    public enum MatchingQueueFlags
    {
        None            = 0x00,
        GroupIsQueued   = 0x04,
        AsMercenary     = 0x20,
        SoloMatch       = 0x80,                      
        Veteran         = 0x100,
    }
}
