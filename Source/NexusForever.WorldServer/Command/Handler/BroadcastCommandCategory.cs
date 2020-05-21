using NexusForever.Shared.Network;
using NexusForever.WorldServer.Command.Context;
using NexusForever.WorldServer.Command.Convert;
using NexusForever.WorldServer.Command.Static;
using NexusForever.WorldServer.Game.RBAC.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Command.Handler
{
    [Command(Permission.Broadcast, "A collection of commands to broadcast server wide messages.", "broadcast")]
    public class BroadcastCommandCategory : CommandCategory
    {
        [Command(Permission.BroadcastMessage, "Broadcast message to all players on the server.", "message")]
        public void HandleBroadcastMessage(ICommandContext context,
            [Parameter("Tier of the message being broadcast.", ParameterFlags.None, typeof(EnumParameterConverter<BroadcastTier>))]
            BroadcastTier tier,
            [Parameter("Message to broadcast.")]
            string message)
        {
            foreach (WorldSession session in NetworkManager<WorldSession>.Instance.GetSessions())
            {
                session.EnqueueMessageEncrypted(new ServerRealmBroadcast
                {
                    Tier    = tier,
                    Message = message
                });
            }
        }
    }
}
