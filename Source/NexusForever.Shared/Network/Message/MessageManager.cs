using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NexusForever.Shared.Network.Message
{
    public delegate void MessageHandlerDelegate(NetworkSession session, IReadable message);

    public sealed class MessageManager : AbstractManager<MessageManager>
    {

        private delegate IReadable MessageFactoryDelegate();

        private ImmutableDictionary<GameMessageOpcode, MessageFactoryDelegate> clientMessageFactories;
        private ImmutableDictionary<Type, GameMessageOpcode> serverMessageOpcodes;

        private ImmutableDictionary<GameMessageOpcode, MessageHandlerDelegate> clientMessageHandlers;

        private MessageManager()
        {
        }

        public override MessageManager Initialise()
        {
            InitialiseMessages();
            InitialiseMessageHandlers();
            return Instance;
        }

        private void InitialiseMessages()
        {
            var messageFactories = new Dictionary<GameMessageOpcode, MessageFactoryDelegate>();
            var messageOpcodes   = new Dictionary<Type, GameMessageOpcode>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes().Concat(Assembly.GetEntryAssembly()?.GetTypes()!))
            {
                MessageAttribute attribute = type.GetCustomAttribute<MessageAttribute>();
                if (attribute == null)
                    continue;

                if (typeof(IReadable).IsAssignableFrom(type))
                {
                    NewExpression @new = Expression.New(type.GetConstructor(Type.EmptyTypes)!);
                    messageFactories.Add(attribute.Opcode, Expression.Lambda<MessageFactoryDelegate>(@new).Compile());
                }
                if (typeof(IWritable).IsAssignableFrom(type))
                    messageOpcodes.Add(type, attribute.Opcode);
            }

            clientMessageFactories = messageFactories.ToImmutableDictionary();
            serverMessageOpcodes   = messageOpcodes.ToImmutableDictionary();
            Log.Info($"Initialised {clientMessageFactories.Count} message {(clientMessageFactories.Count == 1 ? "factory" : "factories")}.");
            Log.Info($"Initialised {serverMessageOpcodes.Count} message(s).");
        }

        private void InitialiseMessageHandlers()
        {
            var messageHandlers = new Dictionary<GameMessageOpcode, MessageHandlerDelegate>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes().Concat(Assembly.GetEntryAssembly()?.GetTypes()!))
            {
                foreach (MethodInfo method in type.GetMethods())
                {
                    if (method.DeclaringType != type)
                        continue;

                    MessageHandlerAttribute attribute = method.GetCustomAttribute<MessageHandlerAttribute>();
                    if (attribute == null)
                        continue;

                    ParameterExpression sessionParameter = Expression.Parameter(typeof(NetworkSession));
                    ParameterExpression messageParameter = Expression.Parameter(typeof(IReadable));

                    ParameterInfo[] parameterInfo = method.GetParameters();

                    if (method.IsStatic)
                    {
                        #region Debug
                        Debug.Assert(parameterInfo.Length == 2);
                        Debug.Assert(typeof(NetworkSession).IsAssignableFrom(parameterInfo[0].ParameterType));
                        Debug.Assert(typeof(IReadable).IsAssignableFrom(parameterInfo[1].ParameterType));
                        #endregion

                        MethodCallExpression call = Expression.Call(method,
                            Expression.Convert(sessionParameter, parameterInfo[0].ParameterType),
                            Expression.Convert(messageParameter, parameterInfo[1].ParameterType));

                        Expression<MessageHandlerDelegate> lambda =
                            Expression.Lambda<MessageHandlerDelegate>(call, sessionParameter, messageParameter);

                        messageHandlers.Add(attribute.Opcode, lambda.Compile());
                    }
                    else
                    {
                        #region Debug
                        Debug.Assert(parameterInfo.Length == 1);
                        Debug.Assert(typeof(NetworkSession).IsAssignableFrom(type));
                        Debug.Assert(typeof(IReadable).IsAssignableFrom(parameterInfo[0].ParameterType));
                        #endregion

                        MethodCallExpression call = Expression.Call(
                            Expression.Convert(sessionParameter, type),
                            method,
                            Expression.Convert(messageParameter, parameterInfo[0].ParameterType));

                        Expression<MessageHandlerDelegate> lambda =
                            Expression.Lambda<MessageHandlerDelegate>(call, sessionParameter, messageParameter);

                        messageHandlers.Add(attribute.Opcode, lambda.Compile());
                    }
                }
            }

            clientMessageHandlers = messageHandlers.ToImmutableDictionary();
            Log.Info($"Initialised {clientMessageHandlers.Count} message handler(s).");
        }

        public IReadable GetMessage(GameMessageOpcode opCode) =>
            clientMessageFactories.TryGetValue(opCode, out MessageFactoryDelegate factory)
                ? factory.Invoke() : null;

        public bool GetOpCode(IWritable message, out GameMessageOpcode opCode) => serverMessageOpcodes.TryGetValue(message.GetType(), out opCode);

        public MessageHandlerDelegate GetMessageHandler(GameMessageOpcode opCode) =>
            clientMessageHandlers.TryGetValue(opCode, out MessageHandlerDelegate handler)
                ? handler : null;
    }
}
