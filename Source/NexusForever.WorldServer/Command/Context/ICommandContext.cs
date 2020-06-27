using System.Collections.Immutable;
using NexusForever.Shared.GameTable.Static;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.RBAC.Static;

namespace NexusForever.WorldServer.Command.Context
{
    public interface ICommandContext
    {
        WorldEntity Invoker { get; }
        WorldEntity Target { get; }

        Language Language { get; }
        ImmutableHashSet<Permission> Permissions { get; }

        /// <summary>
        /// Send information message containing the supplied string.
        /// </summary>
        void SendMessage(string message);

        /// <summary>
        /// Send error message containing the supplied string.
        /// </summary>
        void SendError(string message);

        /// <summary>
        /// Return <see cref="WorldEntity"/> target, if no target is present return the <see cref="WorldEntity"/> invoker.
        /// </summary>
        T GetTargetOrInvoker<T>() where T : WorldEntity;
    }
}
