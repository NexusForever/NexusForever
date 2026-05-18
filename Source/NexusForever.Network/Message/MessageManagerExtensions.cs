using System.Reflection;
using NexusForever.Shared;

namespace NexusForever.Network.Message
{
    public static class MessageManagerExtensions
    {
        public static void RegisterNetworkManagerMessagesAndHandlers(this IMessageManager messageManager)
        {
            TypeWalker.Walk<MessageAttribute>(Assembly.GetExecutingAssembly(), (t, a) =>
            {
                messageManager.RegisterMessage(t);
            });

            foreach (Type type in Assembly.GetExecutingAssembly().GetNetworkHandlerTypes())
                messageManager.RegisterMessageHandler(type);
        }
    }
}
