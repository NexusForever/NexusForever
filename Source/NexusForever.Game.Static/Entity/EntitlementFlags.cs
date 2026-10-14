namespace NexusForever.Game.Static.Entity
{
    [Flags]
    public enum EntitlementFlags
    {
        None      = 0,
        Character = 1,
        Hidden    = 2  // not shown by the client, still granted and counted
    }
}
