using System.Reflection;
using NexusForever.Network.Message;
using NexusForever.Shared;

namespace NexusForever.Network.Auth.Message
{
    public static class MessageManagerExtensions
    {
        public static void RegisterNetworkManagerAuthMessages(this IMessageManager messageManager)
        {
            TypeWalker.Walk<MessageAttribute>(Assembly.GetExecutingAssembly(), (t, a) =>
            {
                messageManager.RegisterMessage(t);
            });
        }
    }
}
