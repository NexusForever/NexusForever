namespace NexusForever.Network.Session
{
    public delegate void NewSessionEvent<T>(T socket) where T : INetworkSession;
}
