namespace NexusForever.IO.Format.NexusForever.Map.Static
{
    [Flags]
    public enum ZoneBoundFlags
    {
        None       = 0x00,
        RoadMarch  = 0x01,
        WorldZone1 = 0x08,
        WorldZone2 = 0x10,
        WorldZone0 = 0x20
    }
}
