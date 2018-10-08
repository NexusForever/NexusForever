namespace NexusForever.StsServer.Network.Message
{
    public class MessageHandlerInfo
    {
        public MessageHandlerDelegate Delegate { get; }
        public SessionState? State { get; }

        public MessageHandlerInfo(MessageHandlerDelegate @delegate, SessionState? state = null)
        {
            Delegate = @delegate;
            State    = state;
        }
    }
}
