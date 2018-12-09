namespace NexusForever.WorldServer.Game.Entity.Static
{
    public enum PvPFlag
    {
        Disabled    = 0,
        Enabled     = 1,
        Forced      = 2, // Seems to be forced to server settings. Toggle "on" is disabled on PvE server. Possibly a city thing?
        Unknown     = 6 // Same deal as PvPFlag.Forced.
    }
}
