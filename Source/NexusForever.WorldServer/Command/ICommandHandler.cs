using System.Text;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Static;

namespace NexusForever.WorldServer.Command
{
    public interface ICommandHandler
    {
        /// <summary>
        /// Return help text that provides a brief summery of the <see cref="ICommandHandler"/>.
        /// </summary>
        void GetHelp(StringBuilder builder, ICommandContext context, bool detailed);

        /// <summary>
        /// Returns if the supplied <see cref="ICommandContext"/> can invoke <see cref="ICommandHandler"/>.
        /// </summary>
        CommandResult CanInvoke(ICommandContext context);

        /// <summary>
        /// Invoke <see cref="ICommandHandler"/> with the supplied <see cref="ICommandContext"/> and <see cref="ParameterQueue"/>.
        /// </summary>
        CommandResult Invoke(ICommandContext context, ParameterQueue queue);

        /// <summary>
        /// Invoke help for <see cref="ICommandHandler"/> with the supplied <see cref="ICommandContext"/> and <see cref="ParameterQueue"/>.
        /// </summary>
        /// <remarks>
        /// This will display help text if the <see cref="ICommandContext"/> has permission to invoke this <see cref="ICommandHandler"/>.
        /// </remarks>
        CommandResult InvokeHelp(ICommandContext context, ParameterQueue queue);
    }
}
