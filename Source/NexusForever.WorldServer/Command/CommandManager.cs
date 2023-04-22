﻿using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Hosting.Systemd;
using Microsoft.Extensions.Hosting.WindowsServices;
using NexusForever.Shared;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Convert;
using NexusForever.WorldServer.Command.Static;
using NLog;

namespace NexusForever.WorldServer.Command
{
    public sealed class CommandManager : Singleton<CommandManager>, ICommandManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private delegate IParameterConvert ParameterConverterFactoryDelegate();
        private ImmutableDictionary<Type, ParameterConverterFactoryDelegate> converterFactories;
        private ImmutableDictionary<Type, Type> defaultConverterFactories;

        private ImmutableDictionary<string, ICommandHandler> handlers;

        private readonly ConcurrentQueue<PendingCommand> pendingCommands = new();

        private Thread commandThread;
        private readonly ManualResetEventSlim waitHandle = new();

        private volatile CancellationTokenSource cancellationToken;

        /// <summary>
        /// Initialise <see cref="ICommandManager"/> and any related resources.
        /// </summary>
        public void Initialise()
        {
            if (cancellationToken != null)
                throw new InvalidOperationException();

            log.Info("Initialising command mananger...");

            BuildConverters();
            InitialiseHandlers();

            if (!WindowsServiceHelpers.IsWindowsService() && !SystemdHelpers.IsSystemdService())
                InitialiseCommandThread();
        }

        private void BuildConverters()
        {
            log.Info("Initialising command parameters converters...");

            var factoryBuilder = ImmutableDictionary.CreateBuilder<Type, ParameterConverterFactoryDelegate>();
            var defaultBuilder = ImmutableDictionary.CreateBuilder<Type, Type>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!typeof(IParameterConvert).IsAssignableFrom(type))
                    continue;

                ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                if (constructor == null)
                    continue;

                // generic converters are a special case that need handling from the parameter attribute
                // at this point we have no idea what the generic parameter replacements should be
                if (type.IsGenericTypeDefinition)
                    continue;

                NewExpression @new = Expression.New(constructor);
                factoryBuilder.Add(type, Expression.Lambda<ParameterConverterFactoryDelegate>(@new).Compile());

                // ConvertAttribute will specify the default type the converter is for
                ConvertAttribute attribute = type.GetCustomAttribute<ConvertAttribute>(false);
                if (attribute != null)
                    defaultBuilder.Add(attribute.Type, type);
            }

            // special case for generic converters
            foreach (MethodInfo method in Assembly.GetExecutingAssembly()
                .GetTypes()
                .SelectMany(t => t.GetMethods()))
            {
                CommandAttribute attribute = method.GetCustomAttribute<CommandAttribute>();
                if (attribute == null)
                    continue;

                foreach (ParameterInfo parameter in method.GetParameters())
                {
                    ParameterAttribute parameterAttribute = parameter.GetCustomAttribute<ParameterAttribute>();
                    if (parameterAttribute == null)
                        continue;

                    if (!parameterAttribute.Converter?.IsGenericType ?? true)
                        continue;

                    ConstructorInfo constructor = parameterAttribute.Converter.GetConstructor(Type.EmptyTypes);
                    if (constructor == null)
                        continue;

                    if (factoryBuilder.ContainsKey(parameterAttribute.Converter))
                        continue;

                    NewExpression @new = Expression.New(constructor);
                    factoryBuilder.Add(parameterAttribute.Converter, Expression.Lambda<ParameterConverterFactoryDelegate>(@new).Compile());
                }
            }

            converterFactories        = factoryBuilder.ToImmutable();
            defaultConverterFactories = defaultBuilder.ToImmutable();
        }

        private void InitialiseHandlers()
        {
            log.Info("Initialising command handlers...");

            var builder = ImmutableDictionary.CreateBuilder<string, ICommandHandler>(
                StringComparer.InvariantCultureIgnoreCase);
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                CommandAttribute attribute = type.GetCustomAttribute<CommandAttribute>();
                if (attribute == null)
                    continue;

                // initialise only the parent, the children will be initialised from the parent itself
                if (type.IsNested)
                    continue;

                if (!typeof(CommandCategory).IsAssignableFrom(type))
                    continue;

                CommandCategory category = (CommandCategory)Activator.CreateInstance(type);
                category.Build(attribute);

                foreach (string command in attribute.Commands)
                    builder.Add(command, category);
            }

            handlers = builder.ToImmutable();
        }

        private void InitialiseCommandThread()
        {
            cancellationToken = new CancellationTokenSource();

            commandThread = new Thread(CommandThread);
            commandThread.Start();

            // wait for command thread to start before continuing
            waitHandle.Wait();
        }

        private void CommandThread()
        {
            log.Info("Started command thread.");
            waitHandle.Set();

            while (!cancellationToken.IsCancellationRequested)
            {
                Console.Write(">> ");

                // reading console input like this allows for a clean cancel of the thread
                // using the standard read methods blocks the thread
                var sb = new StringBuilder();
                while (true)
                {
                    while (!Console.KeyAvailable)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return;

                        Thread.Sleep(100);
                    }

                    ConsoleKeyInfo cki = Console.ReadKey();
                    if (cki.Key == ConsoleKey.Enter)
                        break;

                    sb.Append(cki.KeyChar);

                }

                HandleCommandDelay(new ConsoleCommandContext(), sb.ToString());
            }

            log.Info("Stopped command thread.");
        }

        /// <summary>
        /// Shutdown <see cref="ICommandManager"/> and any related resources.
        /// </summary>
        public void Shutdown()
        {
            log.Info("Shutting down command manager...");

            // if server is running as a service the command thread is not started
            // in this case there is no thread to shutdown, just return
            if (cancellationToken == null)
                return;

            cancellationToken.Cancel();

            commandThread.Join();
            commandThread = null;

            cancellationToken = null;
        }

        public void Update(double lastTick)
        {
            // handle any delayed commands on the world thread
            while (pendingCommands.TryDequeue(out PendingCommand command))
                HandleCommand(command.Context, command.CommandText);
        }

        /// <summary>
        /// Return overloaded <see cref="IParameterConvert"/> for supplied <see cref="Type"/>.
        /// </summary>
        public IParameterConvert GetParameterConverter(Type type)
        {
            return converterFactories.TryGetValue(type, out ParameterConverterFactoryDelegate @delegate)
                ? @delegate.Invoke() : null;
        }

        /// <summary>
        /// Return default <see cref="IParameterConvert"/> for supplied <see cref="Type"/>.
        /// </summary>
        public IParameterConvert GetParameterConverterDefault(Type type)
        {
            return defaultConverterFactories.TryGetValue(type, out Type converterType)
                ? GetParameterConverter(converterType) : null;
        }

        /// <summary>
        /// Handle command with supplied <see cref="ICommandContext"/>.
        /// </summary>
        public void HandleCommand(ICommandContext context, string commandText)
        {
            static string GetCommandError(CommandResult result)
            {
                return result switch
                {
                    // generic result for NoPermission and NoCommand to prevent command scraping
                    CommandResult.NoPermission      => "Unable to invoke command, it's either an invalid command or you don't have permission to access it!",
                    CommandResult.NoCommand         => "Unable to invoke command, it's either an invalid command or you don't have permission to access it!",
                    CommandResult.InvalidParameters => "Invalid parameters supplied to command, see help for more information!",
                    CommandResult.InvalidContext    => "Invalid context for this command!",
                    CommandResult.InvalidTarget     => "Invalid target for this command!",
                    _                               => "Something went wrong :("
                };
            }

            CommandResult result = HandleCommandInternal(context, commandText);
            if (result != CommandResult.Ok)
                context.SendError(GetCommandError(result));
        }

        private CommandResult HandleCommandInternal(ICommandContext context, string commandText)
        {
            string[] parameters = commandText.Split(' ');

            var queue = new ParameterQueue(parameters);
            if (queue.Count == 0)
                return CommandResult.InvalidParameters;

            string command = queue.Dequeue();
            if (!handlers.TryGetValue(command, out ICommandHandler handler))
                return CommandResult.NoCommand;

            return handler.Invoke(context, queue);
        }

        /// <summary>
        /// Handle command with supplied <see cref="ICommandContext"/>.
        /// </summary>
        /// <remarks>
        /// This will queue the command to be invoked on the world thread at the end of an update.
        /// This is useful when a command isn't invoked from the world thread in the first place (console, websocket, ect...) and needs to be thread safe.
        /// </remarks>
        public void HandleCommandDelay(ICommandContext context, string commandText)
        {
            pendingCommands.Enqueue(new PendingCommand(context, commandText));
        }

        /// <summary>
        /// Handle command with supplied <see cref="ICommandContext"/>.
        /// </summary>
        public void HandleHelp(ICommandContext context, ParameterQueue queue)
        {
            static string GetCommandError(CommandResult result)
            {
                return result switch
                {
                    // generic result for NoPermission and NoCommand to prevent command scraping
                    CommandResult.NoPermission      => "No help exists for this command, it's either invalid an command or you don't have permission to access it!",
                    CommandResult.NoCommand         => "No help exists for this command, it's either invalid an command or you don't have permission to access it!",
                    _                               => "Something went wrong :("
                };
            }

            CommandResult result = HandleHelpInternal(context, queue);
            if (result != CommandResult.Ok)
                context.SendError(GetCommandError(result));
        }

        private CommandResult HandleHelpInternal(ICommandContext context, ParameterQueue queue)
        {
            if (queue.Count == 0)
            {
                // if no commands are supplied, show root categories
                var builder = new StringBuilder();
                builder.AppendLine("-----------------------------------------------");
                builder.AppendLine($"Showing help for: {queue.BreadcrumbTrail}");

                foreach ((string _, ICommandHandler rootHandler) in handlers
                    .OrderBy(p => p.Key))
                {
                    if (rootHandler.CanInvoke(context) != CommandResult.Ok)
                        continue;

                    builder.Append("Category: ");
                    rootHandler.GetHelp(builder, context, false);
                }

                context.SendMessage(builder.ToString());
                return CommandResult.Ok;
            }

            string command = queue.Dequeue();
            if (!handlers.TryGetValue(command, out ICommandHandler handler))
                return CommandResult.NoCommand;

            return handler.InvokeHelp(context, queue);
        }
    }
}
