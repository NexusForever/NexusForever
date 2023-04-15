using System;
using System.Collections.Immutable;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Social;
using NexusForever.Game.Static;
using NexusForever.Game.Static.RBAC;
using NexusForever.Game.Static.Social;
using NexusForever.WorldServer.Network;

namespace NexusForever.WorldServer.Command.Context
{
    public class WorldSessionCommandContext : ICommandContext
    {
        public IWorldEntity Invoker { get; }
        public IWorldEntity Target { get; }

        public Language Language { get; }
        public ImmutableHashSet<Permission> Permissions { get; }

        private readonly IWorldSession session;

        /// <summary>
        /// Create a new <see cref="WorldSessionCommandContext"/> with the <see cref="Permission"/>'s from the invokers <see cref="Role"/>'s.
        /// </summary>
        public WorldSessionCommandContext(IWorldSession session, IWorldEntity target)
        {
            this.session = session;

            Invoker      = session.Player;
            Target       = target;
            Language     = Language.English; // pull this from hello packet received from client?
            Permissions  = session.Account.RbacManager.GetPermissions();
        }

        /// <summary>
        /// Send information message containing the supplied string.
        /// </summary>
        public void SendMessage(string message)
        {
            SendText(message);
        }

        /// <summary>
        /// Send error message containing the supplied string.
        /// </summary>
        public void SendError(string message)
        {
            SendText(message, "Error");
        }

        /// <summary>
        /// Return <see cref="IWorldEntity"/> target, if no target is present return the <see cref="IWorldEntity"/> invoker.
        /// </summary>
        public T GetTargetOrInvoker<T>() where T : IWorldEntity
        {
            return (T)(Target ?? Invoker);
        }

        private void SendText(string text, string name = "")
        {
            foreach (string line in text.Trim().Split(Environment.NewLine))
            {
                var builder = new ChatMessageBuilder
                {
                    Type     = ChatChannelType.System,
                    FromName = name,
                    Text     = line,
                    Guid     = Invoker.Guid
                };
                session.EnqueueMessageEncrypted(builder.Build());
            }
        }
    }
}
