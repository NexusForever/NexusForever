using System;

namespace NexusForever.StsServer.Network.Message
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MessageHandlerAttribute : Attribute
    {
        public string Uri { get; }
        public SessionState State { get; }

        public MessageHandlerAttribute(string uri, SessionState state)
        {
            Uri   = uri;
            State = state;
        }
    }
}
