using System;
using System.Collections.Immutable;
using NexusForever.Shared.GameTable.Static;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.RBAC.Static;
using NexusForever.WorldServer.Game.Social.Static;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Command.Context
{
    public class WorldSessionCommandContext : ICommandContext
    {
        public WorldEntity Invoker { get; }
        public WorldEntity Target { get; }

        public Language Language { get; }
        public ImmutableHashSet<Permission> Permissions { get; }

        /// <summary>
        /// Create a new <see cref="WorldSessionCommandContext"/> with the <see cref="Permission"/>'s from the invokers <see cref="Role"/>'s.
        /// </summary>
        public WorldSessionCommandContext(Player invoker, WorldEntity target)
        {
            Invoker     = invoker;
            Target      = target;
            Language    = Language.English; // pull this from hello packet received from client?
            Permissions = invoker.Session?.AccountRbacManager.GetPermissions();
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
        /// Return <see cref="WorldEntity"/> target, if no target is present return the <see cref="WorldEntity"/> invoker.
        /// </summary>
        public T GetTargetOrInvoker<T>() where T : WorldEntity
        {
            return (T)(Target ?? Invoker);
        }

        private void SendText(string text, string name = "")
        {
            Player player = (Player)Invoker;
            foreach (string line in text.Trim().Split(Environment.NewLine))
            {
                player.Session.EnqueueMessageEncrypted(new ServerChat
                {
                    Guid    = player.Guid,
                    Channel = ChatChannelType.System,
                    Name    = name,
                    Text    = line
                });
            }
        }
    }
}
