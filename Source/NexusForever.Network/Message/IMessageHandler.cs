namespace NexusForever.Network.Message
{
    public interface IMessageHandler<in TSession, in TMessage>
        where TSession : IGameSession
        where TMessage : IReadable
    {
        void HandleMessage(TSession session, TMessage packet);
    }
}
