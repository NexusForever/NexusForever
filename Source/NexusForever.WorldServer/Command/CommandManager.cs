using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Command.Handler;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using NLog;

namespace NexusForever.WorldServer.Command
{
    public static class CommandManager
    {
        private static readonly List<ICommandHandler> commandHandlers = new List<ICommandHandler>();
        private static ILogger Logger { get; } = LogManager.GetCurrentClassLogger();

        public static void Initialise()
        {
            Type[] types = typeof(CommandManager).Assembly.GetTypes().Where(i =>
                    typeof(ICommandHandler).IsAssignableFrom(i) &&
                    i.GetConstructors().Any(x => x.GetParameters().Length == 0 && i.IsPublic) && !i.IsAbstract &&
                    i.IsClass)
                .ToArray();
            foreach (Type type in types)
                commandHandlers.Add((ICommandHandler) Activator.CreateInstance(type));
            Logger.Info("Initialised {0} command handlers.", commandHandlers.Count);
        }

        public static IEnumerable<ICommandHandler> GetCommandHandlers()
        {
            return commandHandlers.OrderBy(i => i.Order);
        }

        public static bool HandleCommand(WorldSession session, string commandText, bool isFromChat, IEnumerable<ChatFormat> chatLinks = null)
        {
            return HandleCommand(new WorldSessionCommandContext(session), commandText, isFromChat, chatLinks);
        }

        public static bool HandleCommand(CommandContext context, string commandText, bool isFromChat, IEnumerable<ChatFormat> chatLinks = null)
        {
            return HandleCommandAsync(context, commandText, isFromChat, chatLinks).GetAwaiter().GetResult();
        }

        public static async Task<bool> HandleCommandAsync(CommandContext session, string commandText, bool isFromChat, IEnumerable<ChatFormat> chatLinks = null)
        {
            if (isFromChat)
            {
                if (!commandText.StartsWith("!"))
                    return false;

                commandText = commandText.Substring(1);
            }

            foreach (ICommandHandler command in GetCommandHandlers())
            {
                if (!await command.HandlesAsync(session, commandText, chatLinks))
                    continue;

                await command.HandleAsync(session, commandText, chatLinks);
                return true;
            }

            return false;
        }
    }
}
