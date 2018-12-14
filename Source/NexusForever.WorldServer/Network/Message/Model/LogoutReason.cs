namespace NexusForever.WorldServer.Network.Message.Model
{

    public enum LogoutReason : byte
    {
        None = 0,
        Kicked = 6,
        Idle = 7,
        UserServerConnectionLost = 8,
        AuthServerConnectionLost = 12,
        WorldServerConnectionLost = 15,
        GameTimeExhausted = 16,
        AccountDisconnected = 19,
        DatabaseTransactionFailure = 20,
        GenericError = 31
    }
}
