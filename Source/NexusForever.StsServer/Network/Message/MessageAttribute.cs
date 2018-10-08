using System;

namespace NexusForever.StsServer.Network.Message
{
    public class MessageAttribute : Attribute
    {
        public string Uri { get; }

        public MessageAttribute(string uri)
        {
            Uri = uri;
        }
    }
}
