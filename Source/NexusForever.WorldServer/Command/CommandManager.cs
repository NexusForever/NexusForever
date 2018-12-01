using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NexusForever.Shared.Configuration;
using NexusForever.WorldServer.Command.Handler;
using NexusForever.WorldServer.Network;
using Microsoft.Extensions.DependencyInjection;
using NexusForever.WorldServer.Command.Contexts;

namespace NexusForever.WorldServer.Command
{
    public static class CommandManager
    {

        private static IServiceProvider Services => DependencyInjection.ServiceProvider;
        public static void Initialise()
        {
            // Force init now for singletons.
            using (var scope = Services.CreateScope())
            {
                scope.ServiceProvider.GetServices<ICommandHandler>();
            }
        }

        public static bool HandleCommand(WorldSession session, string commandText, bool isFromChat)
        {
            return HandleCommand(new WorldSessionCommandContext(session), commandText, isFromChat);
        }
        public static bool HandleCommand(CommandContext session, string commandText, bool isFromChat)
        {
            if (isFromChat)
            {
                if (!commandText.StartsWith("!"))
                {
                    return false;
                }

                commandText = commandText.Substring(1);
            }

            using (var scope = Services.CreateScope())
            {
                foreach (var command in scope.ServiceProvider.GetServices<ICommandHandler>().OrderBy(i => i.Order))
                {
                    if (command.Handles(session, commandText))
                    {
                        command.Handle(session, commandText);
                        return true;
                    }
                }
            }

            return false;
        }

        public static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<ICommandHandler, MountCommandHandler>()
                .AddSingleton<ICommandHandler, AccountCommandHandler>()
                .AddSingleton<ICommandHandler, TeleportHandler>()
                .AddSingleton<ICommandHandler, HelpCommandHandler>();
        }
    }
}
