using System.Collections.Immutable;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static;
using NexusForever.Game.Static.RBAC;

namespace NexusForever.WorldServer.Command.Context
{
    public interface ICommandContext
    {
        IWorldEntity Invoker { get; }
        IWorldEntity Target { get; }

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
        /// Return <see cref="IWorldEntity"/> target, if no target is present return the <see cref="IWorldEntity"/> invoker.
        /// </summary>
        T GetTargetOrInvoker<T>() where T : IWorldEntity;
    }
}
