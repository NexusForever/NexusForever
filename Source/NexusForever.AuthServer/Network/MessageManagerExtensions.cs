using System;
using System.Reflection;
using NexusForever.Network;
using NexusForever.Network.Message;

namespace NexusForever.AuthServer.Network
{
    public static class MessageManagerExtensions
    {
        public static void RegisterNetworkManagerAuthHandlers(this IMessageManager messageManager)
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetNetworkHandlerTypes())
                messageManager.RegisterMessageHandler(type);
        }
    }
}
