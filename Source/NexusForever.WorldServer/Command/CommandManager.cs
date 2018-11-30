using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NexusForever.WorldServer.Network;

namespace NexusForever.WorldServer.Command
{
    public delegate void CommandHandlerDelegate(WorldSession session, string[] parameters);

    public static class CommandManager
    {
        private static ImmutableDictionary<string, CommandHandlerDelegate> commandHandlers;

        public static void Initialise()
        {
            var handlers = new Dictionary<string, CommandHandlerDelegate>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (MethodInfo method in type.GetMethods())
                {
                    IEnumerable<CommandHandlerAttribute> attributes = method.GetCustomAttributes<CommandHandlerAttribute>();
                    foreach(CommandHandlerAttribute att in attributes)
                    {
                        ParameterInfo[] parameterInfo = method.GetParameters();

                        #region Debug
                        Debug.Assert(parameterInfo.Length == 2);
                        Debug.Assert(typeof(WorldSession) == parameterInfo[0].ParameterType);
                        Debug.Assert(typeof(string[]) == parameterInfo[1].ParameterType);
                        #endregion

                        handlers.Add(att.Command, (CommandHandlerDelegate)Delegate.CreateDelegate(typeof(CommandHandlerDelegate), method));
                    }
                }
            }

            commandHandlers = handlers.ToImmutableDictionary();
        }

        public static void ParseCommand(string value, out string command, out string[] parameters)
        {
            string[] split = value.TrimStart('!').Split(' ');
            command    = split[0];
            parameters = split.Skip(1).ToArray();
        }

        public static CommandHandlerDelegate GetCommandHandler(string command)
        {
            return commandHandlers.TryGetValue(command, out CommandHandlerDelegate handler) ? handler : null;
        }
    }
}
