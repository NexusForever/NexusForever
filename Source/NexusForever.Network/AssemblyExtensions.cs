using System.Reflection;
using NexusForever.Network.Message;

namespace NexusForever.Network
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<Type> GetNetworkHandlerTypes(this Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(t => t.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMessageHandler<,>)));
        }
    }
}
