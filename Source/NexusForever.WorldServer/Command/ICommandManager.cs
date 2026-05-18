using System;
using NexusForever.Shared;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Convert;

namespace NexusForever.WorldServer.Command
{
    public interface ICommandManager : IUpdate
    {
        /// <summary>
        /// Initialise <see cref="ICommandManager"/> and any related resources.
        /// </summary>
        void Initialise();

        /// <summary>
        /// Shutdown <see cref="ICommandManager"/> and any related resources.
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Return overloaded <see cref="IParameterConvert"/> for supplied <see cref="Type"/>.
        /// </summary>
        IParameterConvert GetParameterConverter(Type type);

        /// <summary>
        /// Return default <see cref="IParameterConvert"/> for supplied <see cref="Type"/>.
        /// </summary>
        IParameterConvert GetParameterConverterDefault(Type type);

        /// <summary>
        /// Handle command with supplied <see cref="ICommandContext"/>.
        /// </summary>
        void HandleCommand(ICommandContext context, string commandText);

        /// <summary>
        /// Handle command with supplied <see cref="ICommandContext"/>.
        /// </summary>
        /// <remarks>
        /// This will queue the command to be invoked on the world thread at the end of an update.
        /// This is useful when a command isn't invoked from the world thread in the first place (console, websocket, ect...) and needs to be thread safe.
        /// </remarks>
        void HandleCommandDelay(ICommandContext context, string commandText);

        /// <summary>
        /// Handle command with supplied <see cref="ICommandContext"/>.
        /// </summary>
        void HandleHelp(ICommandContext context, ParameterQueue queue);
    }
}