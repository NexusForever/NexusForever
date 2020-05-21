using System.Collections.Immutable;
using System.IO;
using NexusForever.Shared.GameTable.Static;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.RBAC;
using NexusForever.WorldServer.Game.RBAC.Static;
using NLog;

namespace NexusForever.WorldServer.Command.Context
{
    public class ConsoleCommandContext : ICommandContext
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public WorldEntity Invoker { get; }
        public WorldEntity Target { get; }

        public Language Language { get; } = Language.English;
        public ImmutableHashSet<Permission> Permissions { get; }

        /// <summary>
        /// Create a new <see cref="ConsoleCommandContext"/> with the <see cref="Permission"/>'s from the Console <see cref="Role"/>.
        /// </summary>
        public ConsoleCommandContext()
        {
            // console role needs to exist in order for the console command context to work
            RBACRole role = RBACManager.Instance.GetRole(Role.Console);
            if (role == null)
                throw new InvalidDataException("Console role doesn't exist!");

            Permissions = role.Permissions.Keys.ToImmutableHashSet();
        }

        /// <summary>
        /// Send information message containing the supplied string.
        /// </summary>
        public void SendMessage(string message)
        {
            log.Info(message);
        }

        /// <summary>
        /// Send error message containing the supplied string.
        /// </summary>
        public void SendError(string message)
        {
            log.Error(message);
        }

        /// <summary>
        /// Return <see cref="WorldEntity"/> target, if no target is present return the <see cref="WorldEntity"/> invoker.
        /// </summary>
        public T GetTargetOrInvoker<T>() where T : WorldEntity
        {
            return null;
        }
    }
}
