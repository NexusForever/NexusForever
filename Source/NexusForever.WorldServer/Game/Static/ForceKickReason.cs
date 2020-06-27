namespace NexusForever.WorldServer.Game.Static
{
    public enum ForceKickReason
    {
        GMKick            = 0x0006,
        Inactivity        = 0x0007,
        AuthDisconnect    = 0x0008,
        StsDisconnect     = 0x000C,
        WorldDisconnect   = 0x000F,
        GameTimeExpired   = 0x0010,
        AccountDisconnect = 0x0013,
        TransactionFail   = 0x0014
    }
}
