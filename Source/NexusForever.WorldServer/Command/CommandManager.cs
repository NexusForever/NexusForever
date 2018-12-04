using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NexusForever.Shared.Configuration;
using NexusForever.WorldServer.Command.Attributes;
using NexusForever.WorldServer.Command.Contexts;
using NexusForever.WorldServer.Command.Handler;
using NexusForever.WorldServer.Network;

namespace NexusForever.WorldServer.Command
{
    public static class CommandManager
    {
        private static IServiceProvider Services => DependencyInjection.ServiceProvider;

        public static void Initialise()
        {
            // Force init now for singletons.
            using (IServiceScope scope = Services.CreateScope())
            {
                scope.ServiceProvider.GetServices<ICommandHandler>();
            }
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

            using (IServiceScope scope = Services.CreateScope())
            {
                foreach (ICommandHandler command in scope.ServiceProvider.GetServices<ICommandHandler>()
                    .OrderBy(i => i.Order))
                {
                    if (!await command.HandlesAsync(session, commandText))
                        continue;

                    await command.HandleAsync(session, commandText);
                    return true;
                }
            }

            return false;
        }

        public static void RegisterServices(IServiceCollection services)
        {
            Type[] types = typeof(CommandManager).Assembly.GetTypes().Where(i =>
                typeof(ICommandHandler).IsAssignableFrom(i) && !i.IsAbstract && !i.IsInterface &&
                i.GetGenericArguments().Length == 0).ToArray();

            foreach (Type type in types)
                if (type.GetCustomAttribute<TransientCommandAttribute>() != null)
                    services.AddTransient(typeof(ICommandHandler), type);
                else if (type.GetCustomAttribute<ScopedCommandAttribute>() != null)
                    services.AddScoped(typeof(ICommandHandler), type);
                else
                    services.AddSingleton(typeof(ICommandHandler), type);
        }
    }
}
