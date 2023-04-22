using NexusForever.Network.Sts;

namespace NexusForever.StsServer.Network.Message
{
    public interface IMessageManager
    {
        void Initialise();
        IReadable GetMessage(string uri);
        MessageHandlerInfo GetMessageHandler(string uri);
    }
}