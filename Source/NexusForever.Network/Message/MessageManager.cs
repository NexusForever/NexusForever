using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NexusForever.Network.Session;

namespace NexusForever.Network.Message
{
    public delegate void MessageHandlerDelegate(object handler, INetworkSession session, IReadable message);

    public sealed class MessageManager : IMessageManager
    {
        private readonly Dictionary<GameMessageOpcode, Type> clientMessageTypes = [];
        private readonly Dictionary<Type, GameMessageOpcode> serverMessageTypes = [];
        private readonly Dictionary<GameMessageOpcode, Type> clientMessageHandlerTypes = [];
        private readonly Dictionary<GameMessageOpcode, MessageHandlerDelegate> clientMessageHandlerDelegates = [];

        #region Dependency Injection

        private readonly ILogger<MessageManager> log;

        public MessageManager(
            ILogger<MessageManager> log)
        {
            this.log = log;
        }

        #endregion

        /// <summary>
        /// Register message of <see cref="Type"/>.
        /// </summary>
        public void RegisterMessage(Type type)
        {
            MessageAttribute attribute = type.GetCustomAttribute<MessageAttribute>();
            if (attribute == null)
                return;

            if (typeof(IReadable).IsAssignableFrom(type))
            {
                clientMessageTypes.Add(attribute.Opcode, type);
                log.LogTrace($"Registered client message {type.Name} with opcode {attribute.Opcode}");
            }
            if (typeof(IWritable).IsAssignableFrom(type))
            {
                serverMessageTypes.Add(type, attribute.Opcode);
                log.LogTrace($"Registered server message {type.Name} with opcode {attribute.Opcode}");
            }
        }

        /// <summary>
        /// Register message handler of <see cref="Type"/>.
        /// </summary>
        public void RegisterMessageHandler(Type type)
        {
            foreach (Type interfaceType in type.GetInterfaces()
                .Where(i => i.IsGenericType
                    && i.GetGenericTypeDefinition() == typeof(IMessageHandler<,>)))
            {
                InterfaceMapping map = type.GetInterfaceMap(interfaceType);
                MethodInfo methodInfo = map.TargetMethods[0];

                Type[] types = interfaceType.GetGenericArguments();
                Type sessionParameterType = types[0];
                Type messageParameterType = types[1];

                MessageAttribute attribute = messageParameterType.GetCustomAttribute<MessageAttribute>();
                if (attribute == null)
                    continue;

                ParameterExpression handlerParameter = Expression.Parameter(typeof(object));
                ParameterExpression sessionParameter = Expression.Parameter(typeof(INetworkSession));
                ParameterExpression messageParameter = Expression.Parameter(typeof(IReadable));

                MethodCallExpression call = Expression.Call(
                    Expression.Convert(handlerParameter, type),
                    methodInfo,
                    Expression.Convert(sessionParameter, sessionParameterType),
                    Expression.Convert(messageParameter, messageParameterType));

                Expression<MessageHandlerDelegate> lambda =
                    Expression.Lambda<MessageHandlerDelegate>(call, handlerParameter, sessionParameter, messageParameter);

                clientMessageHandlerTypes.Add(attribute.Opcode, type);
                clientMessageHandlerDelegates.Add(attribute.Opcode, lambda.Compile());
                log.LogTrace($"Registered message handler {type.Name} with opcode {attribute.Opcode}");
            }
        }

        /// <summary>
        /// Get message <see cref="Type"/> for supplied <see cref="GameMessageOpcode"/>.
        /// </summary>
        public Type GetMessageType(GameMessageOpcode opcode)
        {
            return clientMessageTypes.TryGetValue(opcode, out Type type) ? type : null;
        }

        /// <summary>
        /// Get message <see cref="GameMessageOpcode"/> for supplied <see cref="IWritable"/>.
        /// </summary>
        public GameMessageOpcode? GetOpcode(IWritable message)
        {
            return serverMessageTypes.TryGetValue(message.GetType(), out GameMessageOpcode opcode) ? opcode : null;
        }

        /// <summary>
        /// Get message handler <see cref="Type"/> for supplied <see cref="GameMessageOpcode"/>.
        /// </summary>
        public Type GetMessageHandlerType(GameMessageOpcode opcode)
        {
            return clientMessageHandlerTypes.TryGetValue(opcode, out Type type) ? type : null;
        }

        /// <summary>
        /// Get message handler delegate for supplied <see cref="GameMessageOpcode"/>.
        /// </summary>
        public MessageHandlerDelegate GetMessageHandlerDelegate(GameMessageOpcode opcode)
        {
            return clientMessageHandlerDelegates.TryGetValue(opcode, out MessageHandlerDelegate handler) ? handler : null;
        }
    }
}
