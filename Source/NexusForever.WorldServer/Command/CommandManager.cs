using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NexusForever.Shared.Configuration;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Command.Handler;
using NexusForever.WorldServer.Network;
using NLog;

namespace NexusForever.WorldServer.Command
{
    public static class CommandManager
    {
        private static ILogger Logger { get; } = LogManager.GetCurrentClassLogger();
        private static readonly List<ICommandHandler> commandHandlers = new List<ICommandHandler>();
        public static void Initialise()
        {
            Type[] types = typeof(CommandManager).Assembly.GetTypes().Where(i =>
                    typeof(ICommandHandler).IsAssignableFrom(i) &&
                    i.GetConstructors().Any(x => x.GetParameters().Length == 0 && i.IsPublic) && !i.IsAbstract && i.IsClass)
                .ToArray();
            foreach (Type type in types)
                commandHandlers.Add((ICommandHandler)Activator.CreateInstance(type));
            Logger.Info("Initialised {0} command handlers.", commandHandlers.Count);
        }

        public static IEnumerable<ICommandHandler> GetCommandHandlers()
        {
            return commandHandlers.OrderBy(i => i.Order);
        }

        public static bool HandleCommand(WorldSession session, string commandText, bool isFromChat)
        {
            return HandleCommand(new WorldSessionCommandContext(session), commandText, isFromChat);
        }

        public static bool HandleCommand(CommandContext context, string commandText, bool isFromChat)
        {
            return HandleCommandAsync(context, commandText, isFromChat).GetAwaiter().GetResult();
        }

        public static async Task<bool> HandleCommandAsync(CommandContext session, string commandText, bool isFromChat)
        {
            if (isFromChat)
            {
                if (!commandText.StartsWith("!"))
                    return false;

                commandText = commandText.Substring(1);
            }

            foreach (ICommandHandler command in GetCommandHandlers())
            {
                if (!await command.HandlesAsync(session, commandText))
                    continue;

                await command.HandleAsync(session, commandText);
                return true;
            }

            return false;
        }
    }
}
