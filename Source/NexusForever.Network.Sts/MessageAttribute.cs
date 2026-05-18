namespace NexusForever.Network.Sts
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
