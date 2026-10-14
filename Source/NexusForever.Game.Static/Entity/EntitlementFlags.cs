namespace NexusForever.Game.Static.Entity
{
    [Flags]
    public enum EntitlementFlags
    {
        None      = 0,
        Character = 1,
        /// <summary>
        /// Entitlement isn't shown by the client, it is still granted and counted.
        /// </summary>
        Hidden    = 2
    }
}
